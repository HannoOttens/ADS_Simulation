import readin
import numpy as np
import scipy.stats as st
import os
from pathlib import Path
import plotly.graph_objects as go

dir_path = Path(os.path.dirname(os.path.realpath(__file__)))
out_folder = dir_path.parent.joinpath("ADS_Simulation/output")
exp = ["base", "exp1", "exp2","exp3", "exp4", "exp5"]
int_max_value = pow(2, 31) - 1
alpha = 0.05
headers_type = [("average_waiting_time", int),
         ("longest_waiting_time", int),
         ("longest_queue", int),
         ("most_left_waiting", int),
         ("percentage_left_waiting", float),
         ("average_rtt", float),
         ("max_rtt", int),
         ("avg_tram_load", int),
         ("lowest_tram_load", int),
         ("higest_tram_load", int),
         ("total_passengers", int),
         ("average_empty_time", int),
         ("longest_empty_time", int),
         ("total_delay_pr", int),
         ("total_delay_uc", int),
         ("punctuality", float)]
intervals = [(21600, int_max_value), (25200, 68400), (27000, 34200), 
             (34200, 57600), (57600, 64800), (64800, int_max_value)]

tables = dict()

def read_tables(folders):
    for folder in folders:
        exp_folder = out_folder.joinpath(folder)
        tables[folder] = dict()
        for file in os.listdir(exp_folder):
            time_span = (int(file.split('_')[0]), int(file.split('_')[1]))
            tables[folder][time_span] = readin.read_in(exp_folder.joinpath(file))

def set_types():
    for exp in tables.values():
        for table in exp.values():
            for (k,v) in headers_type:
                table.change_type(k, v)


def t_test(sample1, sample2, column_name):
    tests = dict()
    for k, v in tables[sample1].items():
        a, b = v.columns[column_name], tables[sample2][k].columns[column_name]
        N, df = len(a), len(a) - 1
        zipped = zip(a, b)
        diff = list(map(lambda t: t[0] - t[1], zipped))
        a_mean, b_mean, diff_mean = np.mean(a), np.mean(b), np.mean(diff)
        a_std, b_std, diff_std = np.std(a), np.std(b), np.std(diff)
        t_critical = st.t.ppf(1 - (alpha / 2), df)
        stat = diff_std / np.sqrt(N)
        value = t_critical * stat
        interval = (diff_mean - value, diff_mean + value)
        t_obs = diff_mean / stat
        p_value = (1 - st.t.cdf(np.abs(t_obs), df=df)) * 2
        tests[k] = (f"{interval[0]:.2f};{interval[1]:.2f};{p_value:.5f}")
    return tests

def relation(interval):
    if interval[0] > 0:
        return "larger"
    elif interval[1] < 0:
        return "smaller"
    else:
        return "equal"

def confidence_table(sample1, sample2):
    headers = list(map(lambda t: t[0], headers_type))
    data = [intervals]
    for header in headers:
        test = t_test(sample1, sample2, header)
        data.append(list(map(lambda t: test[t], intervals)))
    headers.insert(0, "interval")
    return go.Table(header=dict(values=headers), cells=dict(values=data))


def confidence_table_csv(sample1, sample2, filepath):
    headers = list(map(lambda t: t[0], headers_type))
    data = [list(map(interval_to_string, intervals))]
    for header in headers:
        test = t_test(sample1, sample2, header)
        data.append(list(map(lambda t: test[t], intervals)))
    headers.insert(0, "interval")
    rows = [headers]
    for i in range(0, len(data[0])):
        rows.append(list(map(lambda x: x[i], data)))
    table = readin.Table(rows)
    table.save_as_csv(filepath)

def interval_to_string(interval):
    begin = f"{interval[0]/3600}"
    end = "End" if interval[1] == int_max_value else f"{interval[1]/3600}"
    return begin + "-" + end

def main():
    pair = ("base", "exp1")
    read_tables(exp)
    set_types()
    # table = confidence_table("base", "exp1")
    # fig = go.Figure(data=table)
    # fig.update_layout(width=2500, height=800)
    # fig.show()
    confidence_table_csv(pair[0], pair[1], f"{pair}.csv")


if __name__ == "__main__":
    main()