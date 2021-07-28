# siemens-jwellers

## Folder structure - 
- Models
	- CodeModels
        - Contains models specific to the code as per requirement
	- DBModels
        - Contains models specific to the database
- HelperMethods
- Controllers
- BLL

## Packages used - 
- BCrypt.Net-Next
- EntityFrameworkCore
- EntityFrameworkCore.Tools
- MySql.EntityFrameworkCore
- Newtonsoft.Json

## Controllers
- Customer
	- AddNewCustomer - 
		- POST
		- We will create a new user, iff there is no other user with same email or username
		- We will also create the login record.
		
	- GetAllCustomers - 
		- GET
		- Gets the details of all customers present
		
	- GetCustomerById - 
		- GET
		- Gets the details of customer by id

- Register
	- VerifyLogin - 
		- POST
		- Verifies the given username and password
		- Returns the userid on successfull verification
		
	- ResetLoginCreds - 
		- PUT
		- We take userid of the loggedin user, email, username, password as input
		- We verify if the userid of the loggedin user and userid for the given email are same or not
		- iff yes then we allow to update the username and password

- Invoices
	- AddNewInvoice - 
		- POST
		- Inputs are userId and invoice details
		- First verify the overall details
		- Then get the proper itemtype and weight unit
		- Then get the price based on item type and currency
		- Check if user is priviledged or not
		- Calculate actual price
		- Calculate discount price, if discount is present
		- Add payment status
		- Save new record to the database

	- GetAllInvoices - 
		- GET
		- Gets all the invoices details

	- GetInvoiceById - 
		- GET
		- Gets all the invoices by id

	- GetInvoicesForUserId - 
		- GET
		- Gets all the invoices for userid

- Rates
	- AddRate
		- POST
		- Input is the new rates details
		- Check the data format
		- Check the currency value
		- Check the unit value - as of now, should always be "gram"
		- Check the item type
		- Check if the combination of itemType, unit(which is gram only) and currency, exists or not
			- if yes - then we need to update the existing record
			- if no - create a new record

	- UpdateRate
		- PUT
		- Input is the new rates details
		- Check the data format
		- Check the currency value
		- Check the unit value - as of now, should always be "gram"
		- Check the item type
		- Check if the combination of itemType, unit(which is gram only) and currency, exists or not
			- if yes - then we need to update the existing record
			- if no - create a new record

	- GetAllRates - 
		- GET
		- Gets all the rates details

	- GetRatesById - 
		- GET
		- Gets the details for a particular rate, given proper id
		
	- GetRatesForItemType - 
		- GET
		- Gets the rates for a particular item type
		
	- DeleteRateById -
		- DELETE
		- Deletes a record based on rateId

Note - This is just a POC, we can have many improvements