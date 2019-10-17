import readin
import numpy as np
import scipy.stats as st
import os
from pathlib import Path

dir_path = Path(os.path.dirname(os.path.realpath(__file__)))
out_folder = dir_path.parent.joinpath("ADS_Simulation/output")
exp = ["base", "exp1"]
int_max_value = pow(2, 31) - 1
types = [("average_waiting_time", int),
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

tables = dict()

def read_tables(folders):
    for folder in folders:
        exp_folder = out_folder.joinpath(folder)
        tables[folder] = dict()
        for file in os.listdir(exp_folder):
            time_span = (file.split('_')[0], file.split('_')[1])
            tables[folder][time_span] = readin.read_in(exp_folder.joinpath(file))

def set_types():
    for exp in tables.values():
        for table in exp.values():
            for (k,v) in types:
                table.change_type(k, v)


def t_test(sample1, sample2, column_name):
    tests = dict()
    for k, v in tables[sample1].items():
        a, b = tables[sample2][k].columns[column_name], v.columns[column_name]
        N, p, df = len(a), 0.95, len(a) - 1
        zipped = zip(a, b)
        diff = list(map(lambda t: t[0] - t[1], zipped))
        a_mean, b_mean, diff_mean = np.mean(a), np.mean(b), np.mean(diff)
        a_std, b_std, diff_std = np.std(a), np.std(b), np.std(diff)
        t_critical = st.t.ppf(p, df)
        stat = t_critical * (diff_std / np.sqrt(N))
        interval = (diff_mean - stat, diff_mean + stat)
        tests[k] = interval
    return tests

def main():
    read_tables(exp)
    set_types()
    print(t_test("base", "exp1", "total_delay_uc"))
    print(t_test("base", "exp1", "total_delay_pr"))


if __name__ == "__main__":
    main()