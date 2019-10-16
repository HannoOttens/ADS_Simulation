import readin

data_table_in = readin.read_in("in_dist.csv")
data_table_out = readin.read_in("out_dist.csv")

print(data_table_in.most_common_value('dist'))
print(data_table_out.most_common_value('dist'))
print(data_table_in.most_common_value('param'))
print(data_table_out.most_common_value('param'))