import readin
import os

target_folder = 'Multi state data/'
param = {
    'f': [14, 16, 18, 20, 22],
    'uc': ['True','False'], 
    'q': [60, 120, 150, 180, 210, 240, 300]
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

