from tabulate import tabulate

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

# Remove the \n and split
def separate(line):
    return line.replace('\n', '').split(';')

# Read in a file as a table
def read_in(filepath):
    with open(filepath) as f:
        lines = f.readlines()
    data = list(map(separate, lines))

    return Table(data)


# Example
t = read_in("C:/Users/hanno/Google Drive/Vakken/Y4S1/ADS/ADS Simulation Assignment/Code/Python data analysis/Data/artificial-input-data-passengers-01.csv")
t.change_type('Direction', int)
t.change_type('From', float)
t.change_type('To', float)
t.change_type('PassIn', float)
t.change_type('PassOut', float)
t.sort_on('Stop')
print(tabulate(t.rows, headers=t.headers))
