# Stack Overflow Clone

## About
Stack Overflow Clone, as the name implies, is an attempt to replicate the stack overflow.
- Core Features:
	- Ask Questions
		- reply with Answers
			- Answers can be accepted
			- Answers can be upvoted and downvoted
			- Answers can be deleted
			- Answers can be updated
			- Answer credit, displays user and when the post was made
		- Questions can be deleted
		- Questions can be updated
		- Question credit, displays user and when the post was made
	- Trending Questions
		- Based on impression algorithm of reply frequency
	- Infinite scroll
	- Direct redirect search bar
	- Profile page
		- Updatable user details
		- All answers and questions are accessible
		- Karma
	- Public profile page
UI:
![[UI.png]]


## Built using: 
- React/ Vite
- Tailwind CSS
- ASP.NET
- MySQL

## Prerequisites: 
- Docker
- Terminal / Powershell

## How To Run

`sample.env`: 
```
DB_USERNAME={YOUR_DB_USERNAME}  
DB_NAME={YOUR_DB_NAME}  
DB_USER_PASSWORD={YOUR_DB_USER_PASSWORD}  
DB_PORT={YOUR_DB_PORT_HERE}  
DOCKER_SERVER_NAME=db  
LOCAL_SERVER_NAME=localhost  
ISSUING_KEY={JWT_ISSUING_KEY}
```
- From Terminal:
	- clone the directory
	- enter the project directory with `cd stack-overflow-clone`
	- On Macbook:
		- run `touch .env` this will create a file call .env.
			- it might help to have a look at our `sample.env` file 
			  to get an idea of the environment variables you need
			  you can run `vim sample.env` in the same directory to have a look
			  you can exit it with `:q`.
		- run `vim .env` now we can edit the file line by line
		  when done, enter `:wq` which will write and quit the file.
	- On Windows:
		- run `copy con .env` this will let you start writing your environment variable values. I'd recommend opening the sample env file in a separate terminal tab or referring above for the values it should contain.
		- Once you have set all the values, hit `CRTL+Z` to save.
	- Now that our environment variables are done, run `cd Backend` and then run `cd BackendServer` to enter the correct directory.
	- Here, we will run `dotnet ef migrations add InitialCreate` this will create a migration script to run when you run the project.
	- Finally, we can run `docker compose up --build` to get the docker running.
		- It is noteworthy that the way to project is setup on the repo, it expects the backend on port 8080 and the frontend on port 5173. If you wish to change these you need to manually modify the frontend `Dockerfile` and `vite.config.js`, otherwise enjoy! The only port that needs to open on your machine is 5173.
- In an IDE
	- clone the directory
	- Enter the project and create a new `.env` file in the main directory that contains both the `Backend` directory and `Frontend` directory.
	- Copy and paste the values from the `sample.env` into the new `.env` changing anything in `{}`.
	- In your own terminal or the IDE's built in interface, run `cd Backend` and then run `cd BackendServer` and finally run `dotnet ef migrations add InitialCreate` to create a migration.
	- Finally run `docker compose up --build`
		- It is noteworthy that the way to project is setup on the repo, it expects the backend on port 8080 and the frontend on port 5173. If you wish to change these you need to manually modify the frontend `Dockerfile` and `vite.config.js`, otherwise enjoy! The only port that needs to open on your machine is 5173.

## Contributors
- matezalantoth
- qeed97
- BalintCoder