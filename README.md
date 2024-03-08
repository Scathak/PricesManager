The dynamic prices manager. 

Assumptions: all prices are stored in a dictionary with dates as keys. There could be 366 days in a year. A new price can be set every minute for a period of time not exceeding 24 hours or 1440 minutes. A price period length of 0 means no price is used. If such a date is not in the dictionary keys, the default price should be used. If a different currency is needed, a different dictionary should be used. Prices can be accessed with a specific date and time for an individual value, as well as for an entire array of prices per minute.
