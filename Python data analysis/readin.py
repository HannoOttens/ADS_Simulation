from tabulate import tabulate

class Table:
    def __init__(self, data):
        self.headers = data[0]
        self.rows = data[1:]
        self.columns = columize(self.headers, data[1:])
    
    def change_type(self, column, _type):
        if column not in self.headers:
            raise ValueError(f"Header does not exist {column}")
        self.columns[column] = list(map(_type, self.columns[column]))

    def as_arrays(self):
        arr = []
        for header in self.headers:
            arr.append(self.columns[header])
        return arr

def columize(headers, data):
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

def read_in(filepath):
    with open(filepath) as f:
        lines = f.readlines()
    data = list(map(separate, lines))
    
    return Table(data)

print('hi')
t = read_in("C:/Users/hanno/Google Drive/Vakken/Y4S1/ADS/ADS Simulation Assignment/Code/Python data analysis/Data/artificial-input-data-passengers-01.csv")
t.change_type('Direction', int)
t.change_type('From', float)
t.change_type('To', float)
t.change_type('PassIn', float)
t.change_type('PassOut', float)
print(tabulate(t.rows, headers=t.headers))
