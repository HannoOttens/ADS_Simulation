import readin
import datetime
import time
import statistics
import fit
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


# # Example
# data_table = readin.read_in("Data/data bus 12 sept 2017.csv")

# # Remove columns that we dont need
# data_table.remove('lijnnummer')
# data_table.remove('haltenummer')
# data_table.remove('afstand')
# data_table.remove('geplande vertrektijd')
# data_table.remove('geplande aankomsttijd')

# # Clean data
# data_table.replace(' (Utrecht)', '')
# data_table.replace(' (Utrec', '')
# data_table.replace(' (Utr', '')
# data_table.replace('UMC Utrecht', 'UMC')
# data_table.replace('De Kromme Rijn', 'Kromme Rijn')
# data_table.replace('Stadion Galgenwaard', 'Galgenwaard')
# data_table.replace('CS Jaarbeurszijde', 'Centraal Station')

# # Change to correct types
# data_table.change_type('ritnummer', int)
# data_table.change_type('aantal uitstappers', int)
# data_table.change_type('geregistreerde vertrektijd', timeStrToMinutes)
# data_table.change_type('aantal instappers', int)
# data_table.change_type('geregistreerde aankomsttijd', timeStrToMinutes)

# # Combine the columns to get a timestamp on most data
# data_table.merge_columns('geregistreerde vertrektijd',
#                          'geregistreerde aankomsttijd', 'time')
# data_table.delete_rows_where('time', None)

# data_table.save_as_csv('cleaned_data_a.csv')
# data_table = readin.read_in("cleaned_data_b.csv")

# data_table.change_type('ritnummer', int)
# data_table.change_type('aantal uitstappers', int)
# data_table.change_type('time', int)
# data_table.change_type('aantal instappers', int)

# # Some indexing for rows
# hidx_stop = data_table.headers.index('haltenaam')
# hidx_rn = data_table.headers.index('ritnummer')
# hidx_date = data_table.headers.index('datum')
# hidx_in = data_table.headers.index('aantal instappers')
# hidx_out = data_table.headers.index('aantal uitstappers')

# # Merge Rubenslaan and Sterrenwijk
# lr1 = lambda row : row[hidx_stop] == 'Rubenslaan'
# lr2 = lambda row : row[hidx_stop] == 'Sterrenwijk'
# lmatch = lambda r1, r2 : r1[hidx_rn] == r2[hidx_rn] and r1[hidx_date] == r2[hidx_date]
# def combine_stations(r1,r2):
#     r1[hidx_in] += r2[hidx_in]
#     r1[hidx_out] += r2[hidx_out]
#     r1[hidx_stop] = 'Vaartsche Rijn'
# data_table.merge_rows(lr1, lr2, lmatch, combine_stations)

# data_table.save_as_csv('cleaned_data_b.csv')

# data_table = readin.read_in("cleaned_data_b.csv")

# data_table.replace('Rubenslaan', 'Vaartsche Rijn')
# data_table.replace('Sterrenwijk', 'Vaartsche Rijn')

# data_table.save_as_csv('cleaned_data_c.csv')
data_table = readin.read_in("cleaned_data_c.csv")
data_table.replace('Vaarsche Rijn', 'Vaartsche Rijn')
data_table.save_as_csv("cleaned_data_c.csv")

data_table.change_type('ritnummer', int)
data_table.change_type('aantal uitstappers', int)
data_table.change_type('time', int)
data_table.change_type('aantal instappers', int)

# Some header indexes
hidx_stop = data_table.headers.index('haltenaam')
hidx_rn = data_table.headers.index('ritnummer')
hidx_date = data_table.headers.index('datum')
hidx_in = data_table.headers.index('aantal instappers')
hidx_out = data_table.headers.index('aantal uitstappers')

# # Aggregate people who get in
# data_table.aggregate_into(
#     0, lambda v, row: max(0, v + row[hidx_in] - row[hidx_out]), 'passenger_count', 'ritnummer')

# # Calculate % of people who get out
# hidx_count = data_table.headers.index('passenger_count')
# data_table.calc_new(lambda row: 0 if row[hidx_count] ==
#                     0 else row[hidx_out]/row[hidx_count], 'percentage_out')

# data_table.save_as_csv("cleaned_data_d.csv")

# Get the station names
stations = data_table.unique_values_from('haltenaam')
directions = [0, 1]


def to_normal_values(column):
    new_headers = ['stop', 'direction', 'time', 'average',
                   'sd', 'dist', 'param']
    t_result = readin.Table([new_headers])
    for station in stations:
        for direction in directions:
            print(station, direction)
            t2 = data_table.from_filter(
                stationDirectionFilter(station, direction))

            # Bin Dta
            data = t2.bin_time_interval(15, 'time')

            # Row indexes:
            c_idx = t2.headers.index(column)
            d_idx = t2.headers.index('datum')

            # Make new table
            b = readin.Table([new_headers])
            (distrubution, best_params) = (None, None)

            # Insert data
            for interval, rows in data:
                # Gather counts and dates
                counts = []
                for row in rows:
                    counts.append(row[t2.headers.index(column)])
                dates = []
                for row in rows:
                    dates.append(row[t2.headers.index('datum')])
                unique_dates_count = len(set(dates))

                # Get the sd and average.
                if(len(counts) == 0):
                    average = 0
                    sd = 0
                elif(len(counts) == 1):
                    average = counts[0]
                    sd = 0
                else:
                    average = sum(counts) / unique_dates_count
                    sd = statistics.stdev(counts)
                    (distrubution, best_params) = fit.best_fit_distribution(counts)

                b.add_rows(
                    [[station, direction, interval, average, sd, distrubution, str(best_params).replace(',', ' ')]])
            t_result.add_rows(b.rows)
    return t_result


t_result = to_normal_values('aantal uitstappers')
t_result.save_as_csv('out_dist.csv')

t_result = to_normal_values('aantal instappers')
t_result.save_as_csv('in_dist.csv')
