import readin
import datetime
import time
import statistics
from tabulate import tabulate


def timeStrToMinutes(string):
    if(string == ''): 
        return None

    x = time.strptime(string, '%H:%M:%S')
    minute = x.tm_min
    hour = x.tm_hour
    return (60*hour + minute)

def stationDirectionFilter(station, direction):
    def f(headers, row):
        station_index = headers.index('haltenaam')
        direction_index = headers.index('ritnummer')
        return row[station_index] == station \
            and row[direction_index] % 2 == direction
    return f

# Example
data_table = readin.read_in(
    "C:/Users/hanno/Documents/UU/ADS_Simulation/Python data analysis/Data/data bus 12 sept 2017.csv")

# Remove columns that we dont need
data_table.remove('datum')
data_table.remove('lijnnummer')
data_table.remove('haltenummer')
data_table.remove('afstand')
data_table.remove('geplande vertrektijd')
data_table.remove('geplande aankomsttijd')

# Clean data
data_table.replace(' (Utrecht)', '')
data_table.replace(' (Utr', '')
data_table.replace(' (Utrec', '')

data_table.delete_rows_where('haltenaam', 'Rubenslaan')

# Change to correct types
data_table.change_type('ritnummer', int)
data_table.change_type('aantal uitstappers', int)
data_table.change_type('geregistreerde vertrektijd', timeStrToMinutes)
data_table.change_type('aantal instappers', int)
data_table.change_type('geregistreerde aankomsttijd', timeStrToMinutes)
data_table.merge_columns('geregistreerde vertrektijd', 'geregistreerde aankomsttijd', 'time')
data_table.delete_rows_where('time', None)
# print(tabulate(t.rows, headers=t.headers))
# print(tabulate(t.average_per_interval('aantal uitstappers', 15, 'geregistreerde vertrektijd')))
stations = data_table.unique_values_from('haltenaam')
directions = [0,1]

def to_normal_values(column):
    new_headers = ['stop', 'direction', 'time', 'average', 'sd']
    t_result = readin.Table([new_headers])
    for station in stations:
        for direction in directions:
            t2 = data_table.from_filter(stationDirectionFilter(station,direction))

            # Bin Dta
            data = t2.bin_time_interval(
                column, 15, 'time')

            b = readin.Table([new_headers])
            for interval, c in data:
                if(len(c) == 0):
                    average = 0
                    sd = 0
                elif(len(c) == 1):
                    average = c[0]
                    sd = 0
                else:
                    average = statistics.mean(c)
                    sd = statistics.stdev(c)
                b.add_rows([[station, direction, interval, average, sd]])
            # print("================================")
            # print(station, direction)
            # print("================================")
            # for [station, time, avg, sd] in b.rows:
            #     print(f'{station} [{int(time/60)}:{time%60}] Average: {avg}, SD: {sd}')

            t_result.add_rows(b.rows)
    return t_result

t_result = to_normal_values('aantal uitstappers')
t_result.save_as_csv('out.csv')
t_result = to_normal_values('aantal instappers')
t_result.save_as_csv('in.csv')
