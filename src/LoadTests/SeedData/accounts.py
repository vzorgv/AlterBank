
def get_accounts_list():
    return (f"{item:010}" for item in range(1, 10001))

def get_accounts_list_with_balance():
    return ((f"{item:010}", item * 1000) for item in range(1, 10001))

def create_accounts_csv():
    with open("accounts.csv", 'w') as accounts_file:
        firstCompleted = False 
        for item in get_accounts_list():
            account = "\n" + item if firstCompleted else item 
            accounts_file.write(account)
            firstCompleted = True

def create_accounts_with_balance_csv():
    with open("accounts_balance.csv", 'w') as accounts_file:
        firstCompleted = False 
        for item in get_accounts_list_with_balance():
            account = f"{item[0]},{item[1]}"
            account = "\n" + account if firstCompleted else account 
            accounts_file.write(account)
            firstCompleted = True

if __name__ == "__main__":
   create_accounts_csv()
   create_accounts_with_balance_csv()