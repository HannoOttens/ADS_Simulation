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


# Example
data_table = readin.read_in(
    "C:/Users/hanno/Documents/UU/ADS_Simulation/Python data analysis/Data/data bus 12 sept 2017.csv")

# Remove columns that we dont need
data_table.remove('lijnnummer')
data_table.remove('haltenummer')
data_table.remove('afstand')
data_table.remove('geplande vertrektijd')
data_table.remove('geplande aankomsttijd')

# Clean data
data_table.replace(' (Utrecht)', '')
data_table.replace(' (Utrec', '')
data_table.replace(' (Utr', '')
data_table.replace('UMC Utrecht', 'UMC')
data_table.replace('De Kromme Rijn', 'Kromme Rijn')
data_table.replace('Stadion Galgenwaard', 'Galgenwaard')
data_table.replace('CS Jaarbeurszijde', 'Centraal Station')

data_table.replace('Rubenslaan', 'Vaartsche Rijn')
data_table.replace('Sterrenwijk', 'Vaartsche Rijn')

# Change to correct types
data_table.change_type('ritnummer', int)
data_table.change_type('aantal uitstappers', int)
data_table.change_type('geregistreerde vertrektijd', timeStrToMinutes)
data_table.change_type('aantal instappers', int)
data_table.change_type('geregistreerde aankomsttijd', timeStrToMinutes)

# Combine the columns to get a timestamp on most data
data_table.merge_columns('geregistreerde vertrektijd',
                         'geregistreerde aankomsttijd', 'time')
data_table.delete_rows_where('time', None)

# Aggregate people who get in
hidx_in = data_table.headers.index('aantal instappers')
hidx_out = data_table.headers.index('aantal uitstappers')
data_table.sort_on('time')
data_table.aggregate_into(
    0, lambda v, row: v + row[hidx_in] + row[hidx_out], 'passenger_count', 'ritnummer')

# Calculate % of people who get out
hidx_count = data_table.headers.index('passenger_count')
data_table.calc_new(lambda row: 0 if row[hidx_count] ==
                    0 else row[hidx_out]/row[hidx_count], 'percentage_out')

# Get the station names
stations = data_table.unique_values_from('haltenaam')
directions = [0, 1]


def to_normal_values(column):
    new_headers = ['stop', 'direction', 'time', 'average',
                   'sd', 'passenger_count', 'percentage_out']
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
            pc_idx = t2.headers.index('passenger_count')
            pout_idx = t2.headers.index('percentage_out')

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
                    # (distrubution, best_params) = fit.best_fit_distribution(c)

                b.add_rows(
                    [[station, direction, interval, average, sd, row[pc_idx], row[pout_idx]]])
            t_result.add_rows(b.rows)
    return t_result


t_result = to_normal_values('aantal uitstappers')
t_result.save_as_csv('out.csv')

t_result = to_normal_values('aantal instappers')
t_result.save_as_csv('in.csv')
