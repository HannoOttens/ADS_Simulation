import readin

data_table_in = readin.read_in("in.csv")
data_table_out = readin.read_in("out.csv")

data_table_in.replace('Vaarsche Rijn', 'Vaartsche Rijn')
data_table_out.replace('Vaarsche Rijn', 'Vaartsche Rijn')

data_table_in.save_as_csv("in.csv")
data_table_in.save_as_csv("out.csv")


print(data_table_in.most_common_value('distribution'))
print(data_table_out.most_common_value('distribution'))