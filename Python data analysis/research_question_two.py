import readin
import os
import plotly.graph_objects as go
from plotly.colors import n_colors
import numpy as np
from plotly.subplots import make_subplots
import scipy.stats as st
import stats_analyse
import research_question_one as rqo

interval_idx = 2

target_folder = 'Multi state data/q_analysis/'
param = {
    # 'f': [14, 15, 16, 17, 18, 19, 20, 21, 22],
    # 'f': [16],
    'uc': [True, False],
    'q': [100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420],
}
width = 1500
height = 800


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
        colors.insert(0, ['rgb(200, 200, 200)']*len(param[param_y]))

    # built table
    return go.Table(header=dict(values=headers),
                    cells=dict(values=data,
                               fill_color=colors))


columns = [
    ("average_waiting_time", int),
    ("punctuality", float),
    ("longest_waiting_time", int),
    ("total_delay_pr", int),
    ("total_delay_uc", int),
]
intervals = [(25200, 68400), (27000, 34200), (57600, 64800)]
results = [dict(), dict(), dict()]
results_c = []
for i in range(len(intervals)):
    cur_row = []
    for j in range(len(columns)):
        cur_row.append(dict())
    results_c.append(cur_row)

for subdir, dirs, files in os.walk(target_folder):
    # skip root
    if len(dirs) > 0:
        continue
    folder = subdir.replace(target_folder, '')
    args = list(map(lambda arg: arg.split('=')[1], folder.split('-')))
    uc = args[2] == 'True'
    q = int(args[3])

    # loop over correct files
    for idx, (start, end) in enumerate(intervals):
        interval_str = f"{start}_{end}_{folder}.csv"
        file_idx = files.index(interval_str)
        file = files[file_idx]

        # read in
        table = readin.read_in(os.path.join(subdir, file))
        for c, t in columns:
            table.change_type(c, t)

        # average of interesting columns:
        means = []
        for c, _ in columns:
            means.append((c, table.average(c)))

        results[idx][(uc, q)] = means

        # Read in interesting columns in full
        for idx_c in range(len(columns)):
            results_c[idx][idx_c][(uc, q)] = table.columns[columns[idx_c][0]]

# find best key
best_for_c = [pow(2, 30)] * len(columns)
best_for_c_key = [None] * len(columns)
for c_idx in range(len(columns)):
    for key, value in results_c[interval_idx][c_idx].items():
        mean = np.mean(value)
        if best_for_c[c_idx] > mean:
            best_for_c[c_idx] = mean
            best_for_c_key[c_idx] = key

print(best_for_c)
print(best_for_c_key)

# stat comapare all others to that key
tables = []
titles = []
for c_idx in range(len(columns)):
    table = table_array_for('q', 'uc')
    colors = table_array_for('q', 'uc')

    col_dict = dict()
    col_dict["larger"] = 'rgb(255,0,0)'
    col_dict["equal"] = 'rgb(255,255,0)'
    col_dict["smaller"] = 'rgb(0,255,0)'

    for idx_q, q in enumerate(param['q']):
        for idx_uc, uc in enumerate(param['uc']):
            if (uc, q) == best_for_c_key[c_idx]:
                table[idx_q][idx_uc] = "BEST"
                colors[idx_q][idx_uc] = 'rgb(0,255,255)'
            else:
                t_result = rqo.t_test(
                    results_c[interval_idx][c_idx][(uc, q)],
                    results_c[interval_idx][c_idx][best_for_c_key[c_idx]],
                    0.005
                )
                table[idx_q][idx_uc] = stats_analyse.relation(t_result[0])
                colors[idx_q][idx_uc] = col_dict[table[idx_q][idx_uc]]

    tables.append(make_table('q', 'uc', table, colors))
    titles.append(columns[c_idx][0])

rename = {
    'average_waiting_time':  'Average waiting time',
    'punctuality': "Punctuality",
    'longest_waiting_time': 'Longest waiting time',
    'total_delay_pr': 'Total delay at P+R',
    'total_delay_uc': 'Total delay at Utrecht Centraal',
}

titles = list(map(lambda t: rename[t], titles))


fig = make_subplots(
    rows=len(tables),
    cols=1,
    specs=[[{"type": "domain"}]]*len(tables),
    subplot_titles=titles)

for t_idx in range(len(tables)):
    fig.add_trace(tables[t_idx], row=t_idx+1, col=1)

fig.update_layout(width=width, height=height)
fig.show()
