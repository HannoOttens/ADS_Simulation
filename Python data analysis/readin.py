
class Table:
    def __init__(self, data):
        self.headers = data[0]
        self.data = columize(self.headers, data[1:])

def columize(headers, data):
    tbl = {}
    for i in range(len(headers)):
        l = []
        for line in data:
            l.append(line[i])
        tbl[headers[i]] = l
    return tbl

def separate(line):
    return line.split(';')

def read_in(filepath):
    with open(filepath) as f:
        lines = f.readlines()
    
    data = list(map(separate, lines))

    t = Table(data)
    print(t.data)

print('hi')
read_in("C:/Users/hanno/Google Drive/Vakken/Y4S1/ADS/ADS Simulation Assignment/Code/Python data analysis/Data/artificial-input-data-passengers-01.csv")