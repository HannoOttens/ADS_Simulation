import readin
import os
import plotly.graph_objects as go
from plotly.colors import n_colors
import numpy as np
from plotly.subplots import make_subplots

target_folder = 'Multi state data/'
param = {
    'f': [14, 15, 16, 17, 18, 19, 20, 21, 22],
    'uc': [True, False], 
    'q': [60, 90, 120, 150, 180, 210, 240, 270, 300]
}

columns = [("average_waiting_time", int), ("punctuality", float),("total_delay_pr", int),
         ("total_delay_uc", int),]
intervals = [(25200, 68400), (27000, 34200), (57600, 64800)]
results = [dict(), dict(), dict()]
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

        results[idx][(f,uc,q,(start,end))] = means

min_punc_key = None
min_punc = 10000000000000
min_wait_key = None
min_wait = 10000000000000
for k,v in results[0].items():
    if v[0][1] < min_wait:
        min_wait = v[0][1]
        min_wait_key = k
    if v[1][1] < min_punc:
        min_punc = v[1][1]
        min_punc_key = k
    
print(min_punc_key, results[0][min_punc_key])
print(min_wait_key, results[0][min_wait_key])

def table_array_for(name_x, name_y):
    table = []
    for i in range(len(param[name_x])):
        table.append([None]*len(param[name_y]))
    return table

def build_table(uc_switch, val_idx):
    table = table_array_for('f','q')
    for k,v in results[0].items():
        (f,uc,q,_) = k
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
    
    headers = [''] + param['f']
    table.insert(0, param['q'])
    colors.insert(0,['rgb(200, 200, 200)']*len(param['q']))

    return go.Table(header=dict(values=headers),
        cells=dict(values=table,
                    fill_color=colors))


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
