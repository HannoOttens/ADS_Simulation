import readin
from tabulate import tabulate

# Example
t = readin.read_in("C:/Users/hanno/Documents/UU/ADS Simulation/Python data analysis/Data/data bus 12 sept 2017.csv")
# t.change_type('Direction', int)
# t.change_type('From', float)
# t.change_type('To', float)
# t.change_type('PassIn', float)
# t.change_type('PassOut', float)
t.sort_on('ritnummer')
t.remove('lijnnummer')
t.remove('datum')
t.remove('haltenummer')
t.remove('afstand')
t.remove('geplande vertrektijd')
t.remove('geplande aankomsttijd')
t.replace(' (Utrecht)', '')
t.replace(' (Utr', '')
t.replace(' (Utrec', '')
t.delete_rows_where('haltenaam', 'Rubenslaan')
print(tabulate(t.rows, headers=t.headers))
