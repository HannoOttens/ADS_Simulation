import readin

data_table_in = readin.read_in(
    "C:/Users/hanno/Documents/UU/ADS_Simulation/Python data analysis/in.csv")

data_table_out = readin.read_in(
    "C:/Users/hanno/Documents/UU/ADS_Simulation/Python data analysis/out.csv")

print(data_table_in.most_common_value('distribution'))
print(data_table_out.most_common_value('distribution'))