
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

    def replace(self, rval, newval):
        for header in self.headers:
            self.replace_in(header, rval, newval)

    def replace_in(self, header, rval, newval):
        header_index = self.headers.index(header)
        for row in self.rows:
             row[header_index] = row[header_index].replace(rval, newval)
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

