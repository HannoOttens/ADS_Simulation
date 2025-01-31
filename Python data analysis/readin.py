from collections import defaultdict


class Table:
    def __init__(self, data):
        self.headers = data[0]
        self.rows = data[1:]
        self.columns = columnize(self.headers, data[1:])

    # Change the type of a column
    def change_type(self, column, _type):
        if column not in self.headers:
            raise ValueError(f"Header does not exist {column}")
        header_index = self.headers.index(column)
        self.rows = list(map(change_type(_type, header_index), self.rows))
        self.columns = columnize(self.headers, self.rows)

    # Get the columns as arrays instead of a map
    def as_arrays(self):
        arr = []
        for header in self.headers:
            arr.append(self.columns[header])
        return arr

    # Sort on a header
    def sort_on(self, header):
        header_index = self.headers.index(header)
        self.rows.sort(key=sort_on_column(header_index))
        self.columns = columnize(self.headers, self.rows)

    def remove(self, header):
        header_index = self.headers.index(header)
        for row in self.rows:
            del row[header_index]
        del self.columns[header]
        del self.headers[header_index]

    def delete_rows_where(self, header, value):
        header_index = self.headers.index(header)

        to_delete = []
        for idx, row in enumerate(self.rows):
            if row[header_index] == value:
                to_delete.append(idx)

        for s, idx in enumerate(to_delete):
            del self.rows[idx - s]

        self.columns = columnize(self.headers, self.rows)

    def delete_rows_where_lamda(self, lamda):
        to_delete = []
        for idx, row in enumerate(self.rows):
            if lamda(row):
                to_delete.append(idx)

        for s, idx in enumerate(to_delete):
            del self.rows[idx - s]
        self.columns = columnize(self.headers, self.rows)

    def replace(self, rval, newval):
        for header in self.headers:
            self.replace_in(header, rval, newval)

    def replace_in(self, header, rval, newval):
        header_index = self.headers.index(header)
        for row in self.rows:
            row[header_index] = row[header_index].replace(rval, newval)
        self.columns = columnize(self.headers, self.rows)

    def average(self, header):
        column = self.columns[header]
        return sum(column) / len(column)

    def bin_time_interval(self, interval, interval_header):
        # Sort data on header interval
        self.sort_on(interval_header)

        # Get the columns
        interval_column = self.columns[interval_header]

        # Get start point and endpoint
        start = interval_column[0]
        start = start - (start % interval)  # Round to start of interval
        end = interval_column[-1]

        # Make intervals
        index = 0
        max_value = start + interval
        out = []
        while index < len(interval_column):
            curData = []
            while index < len(interval_column) and interval_column[index] < max_value:
                curData.append(self.rows[index])
                index += 1
            out.append((max_value-interval, curData))
            max_value += interval
        return out

    def from_filter(self, filter):
        new_data = [self.headers]
        for row in self.rows:
            if filter(self.headers, row):
                new_data.append(row)
        return Table(new_data)

    def unique_values_from(self, header):
        column = self.columns[header]
        return set(column)

    def add_rows(self, new_rows):
        self.rows.extend(new_rows)
        self.columns = columnize(self.headers, self.rows)

    def save_as_csv(self, filepath):
        outs = [array_to_csv(self.headers)]
        for row in self.rows:
            outs.append(array_to_csv(row))

        out = '\n'.join(outs)
        with open(filepath, 'w') as f:
            f.write(out)

    def merge_columns(self, col_a, col_b, col_new):
        idx_a = self.headers.index(col_a)
        idx_b = self.headers.index(col_b)

        col_data = []
        for val_a, val_b in zip(self.columns[col_a], self.columns[col_b]):
            if val_a is not None:
                col_data.append(val_a)
            elif val_b is not None:
                col_data.append(val_b)
            else:
                col_data.append(None)

        self.remove(col_a)
        self.remove(col_b)
        self.columns[col_new] = col_data
        self.headers = list(self.columns.keys())
        self.rows = rowize(self.columns)

    def most_common_value(self, header):
        column = self.columns[header]
        value_counters = list(set(column))
        value_counters_map = defaultdict(lambda: 0)
        for v in column:
            value_counters_map[v] += 1

        max_val = 0
        v_max = None

        for k, v in value_counters_map.items():
            if v > max_val:
                max_val = v
                v_max = k

        return v_max

    def aggregate_into(self, init_val, lamda, target_column_name, split_on_column):
        self.sort_on('ritnummer')

        col_idx = self.headers.index('haltevolgorde')
        split_index = self.headers.index('ritnummer')
        date_indx = self.headers.index('datum')
        splits = set(zip(self.columns[split_on_column],self.columns['datum']))

        new_col = []
        for idx, (ritnummer, date) in enumerate(splits):
            if idx % 100 == 0: print(str(int(100*idx/len(splits))) + '%')

            split_rows = list(filter(lambda row: row[split_index] == ritnummer and row[date_indx] == date, self.rows))
            split_rows.sort(key=sort_on_column(col_idx))
            v = init_val
            for row in split_rows:
                row.append(v)
                v = lamda(v, row)
                if v < 0:
                    raise v

        self.headers.append(target_column_name)
        self.columns = columnize(self.headers, self.rows)

    def calc_new(self, lamda, target_column_name):
        self.headers.append(target_column_name)
        for row in self.rows:
            row.append(lamda(row))
        self.columns = columnize(self.headers, self.rows)

    def merge_rows(self, lr1, lr2, lmatch, lcombine):
        idx = -1
        while idx < len(self.rows) - 1:
            idx += 1
            r1 = self.rows[idx]

            if(idx%1000 == 0): print(str(idx) + ' of ' + str(len(self.rows)))
            if not lr1(r1): continue
            
            idx2 = 0
            while idx2 < len(self.rows):
                if(idx2 >= idx): break

                r2 = self.rows[idx2]
                if not lr2(r2)\
                        or not lmatch(r1, r2):
                    idx2 += 1
                    continue
                else:
                    lcombine(r1, r2)
                    del self.rows[idx2]
                    idx -= 1
                    break


def array_to_csv(arr):
    strs = [str(v) for v in arr]
    return ','.join(strs)

# Sort key for rows


def sort_on_column(index):
    return lambda v: v[index]

# Change the type of the item at the index


def change_type(_type, index):
    def f(row):
        row[index] = _type(row[index])
        return row
    return f

# Columnize data rows


def columnize(headers, data):
    tbl = {}
    for i in range(len(headers)):
        l = []
        for line in data:
            l.append(line[i])
        tbl[headers[i]] = l
    return tbl


def rowize(columns):
    rows = []
    for tupl in zip(*columns.values()):
        row = [v for v in tupl]
        rows.append(row)
    return rows

# Remove the \n and split


def separate(line):
    return line.replace('\n', '').split(',')

# Read in a file as a table


def read_in(filepath):
    with open(filepath) as f:
        lines = f.readlines()
    data = list(map(separate, lines))

    return Table(data)
