import readin
import os
import plotly.graph_objects as go
from plotly.colors import n_colors
import numpy as np
from plotly.subplots import make_subplots
import scipy.stats as st
import stats_analyse

def table_array_for(name_x, name_y):
    table = []
    for i in range(len(param[name_x])):
        table.append([None]*len(param[name_y]))
    return table


def make_table(param_x, param_y, data, colors=None):
    # add collumns
    headers = [''] + param[param_x]
    data.insert(0, param[param_y])
    if colors:
        colors.insert(0,['rgb(200, 200, 200)']*len(param[param_y]))

    # built table
    return go.Table(header=dict(values=headers),
        cells=dict(values=data,
                    fill_color=colors))


target_folder = 'Multi state data/batch_60/'
param = {
    'f': [14, 15, 16, 17, 18, 19, 20, 21, 22],
    'uc': [True, False], 
    'q': [60, 90, 120, 150, 180, 210, 240, 270, 300]
}

columns = [("average_waiting_time", int), ("punctuality", float),("total_delay_pr", int),
         ("total_delay_uc", int),]
intervals = [(25200, 68400), (27000, 34200), (57600, 64800)]
results = [dict(), dict(), dict()]
results_c0 = [dict(), dict(), dict()]
results_c1 = [dict(), dict(), dict()]
for subdir, dirs, files in os.walk(target_folder):
    # skip root
    if len(dirs) > 0:
        continue
    folder = subdir.replace(target_folder, '')
    args = list(map(lambda arg: arg.split('=')[1], folder.split('-')))
    f = int(args[0])
    uc = args[2] == 'True'
    q = int(args[3])

    # loop over correct files
    for idx, (start,end) in enumerate(intervals):
        interval_str = f"{start}_{end}_{folder}.csv"
        file_idx = files.index(interval_str)
        file = files[file_idx]

        #read in
        table = readin.read_in(os.path.join(subdir,file))
        for c, t in columns:
            table.change_type(c,t)

        # average of interesting columns:
        means = []
        for c, _ in columns:
            means.append((c,table.average(c)))

        results[idx][(f,uc,q)] = means
        results_c0[idx][(f,uc,q)] = table.columns[columns[0][0]]
        results_c1[idx][(f,uc,q)] = table.columns[columns[1][0]]

# def t_test(a,b, alpha):
#     N, df = len(a), len(a) - 1
#     zipped = zip(a, b)
#     diff = list(map(lambda t: t[0] - t[1], zipped))
#     a_mean, b_mean, diff_mean = np.mean(a), np.mean(b), np.mean(diff)
#     a_std, b_std, diff_std = np.std(a), np.std(b), np.std(diff)
#     t_critical = st.t.ppf(1 - (alpha / 2), df)
#     stat = diff_std / np.sqrt(N)
#     value = t_critical * stat
#     interval = (diff_mean - value, diff_mean + value)
#     t_obs = diff_mean / stat
#     p_value = (1 - st.t.cdf(np.abs(t_obs), df=df)) * 2
#     return (interval, p_value < alpha)


# table_c0 = table_array_for('f','q')
# table_c1 = table_array_for('f','q')
# colors_c0 = table_array_for('f','q')
# colors_c1 = table_array_for('f','q')
# col_dict = dict()
# col_dict["larger"] = 'rgb(255,0,0)'
# col_dict["equal"] = 'rgb(255,255,0)'
# col_dict["smaller"] = 'rgb(0,255,0)'

# for idx_f, f in enumerate(param['f']):
#     for idx_q, q in enumerate(param['q']):
#         table_c0[idx_f][idx_q] = stats_analyse.relation(t_test(results_c0[0][(f,True,q)],results_c0[0][(f,False,q)],0.05)[0])
#         table_c1[idx_f][idx_q] = stats_analyse.relation(t_test(results_c1[0][(f,True,q)],results_c1[0][(f,False,q)],0.05)[0])
#         colors_c0[idx_f][idx_q] = col_dict[table_c0[idx_f][idx_q]]
#         colors_c1[idx_f][idx_q] = col_dict[table_c1[idx_f][idx_q]]

# t1= make_table('f','q',table_c0,colors_c0)
# t2= make_table('f','q',table_c1,colors_c1)

# fig = make_subplots(
#     rows=1, 
#     cols=2,
#     specs=[[{"type": "domain"}, {"type": "domain"}]],
#     subplot_titles=["Improvement in waitingtime", "Improvement in punctuality"])

# fig.add_trace(t1, row=1, col=1)
# fig.add_trace(t2, row=1, col=2)

# fig.update_layout(width=1200, height=800)
# fig.show()



###############################
# table stuff
###############################


def build_table(uc_switch, val_idx):
    table = table_array_for('f','q')
    for k,v in results[0].items():
        (f,uc,q) = k
        if (uc_switch and uc) or (not uc_switch and not uc) :
            table[param['f'].index(f)][param['q'].index(q)] = round(v[val_idx][1],2)

    min_val = pow(2,31) - 1
    max_val = 0
    for row in table:
        if min(row) < min_val:
            min_val = min(row)
        if max(row) > max_val:
            max_val = max(row)
    diff = max_val-min_val

    cs = n_colors('rgb(0, 255, 0)', 'rgb(255, 0, 0)', int(diff) +1, colortype='rgb')
    colors = []
    for row in table:
        c_row = []
        for cell in row:
            c_row.append(cs[int(cell-min_val)])
        colors.append(c_row)
    
    return make_table('f','q',table,colors)

names = []
for uc_switch in range(2):
    for v in range(2):
        names.append(f'UC Switch: {bool(uc_switch)} - {"Average Wait Time (s)" if v == 0 else "Punctuality"}')

fig = make_subplots(
    rows=2, 
    cols=2,
    specs=[[{"type": "domain"}, {"type": "domain"}],
           [{"type": "domain"}, {"type": "domain"}]],
    subplot_titles=names)

for row in range(2):
    for column in range(2):
        fig.add_trace(build_table(row, column), row=row+1, col=column+1)

fig.update_layout(width=1200, height=800)
fig.show()
