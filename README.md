# Stack Overflow Clone

## About
Stack Overflow Clone, as the name implies, is an attempt to replicate Stack Overflow.
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

<p align="left">
  <a href="https://react.dev/" target="_blank"><img src="https://img.shields.io/badge/React-61DAFB?style=for-the-badge&logo=react&logoColor=white" alt="React"/></a>
  <a href="https://vitejs.dev/" target="_blank"><img src="https://img.shields.io/badge/Vite-646CFF?style=for-the-badge&logo=vite&logoColor=white" alt="Vite"/></a>
  <a href="https://dotnet.microsoft.com/en-us/" target="_blank"><img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/></a>
  <a href="https://www.mysql.com/" target="_blank"><img src="https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white" alt="MySQL"/></a>
  <a href="https://www.docker.com/" target="_blank"><img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker"/></a>
  <a href="https://developer.mozilla.org/en-US/docs/Web/CSS" target="_blank"><img src="https://img.shields.io/badge/CSS-1572B6?style=for-the-badge&logo=css3&logoColor=white" alt="CSS"/></a>
</p>


## Prerequisites: 
- Docker
- Terminal / Powershell

## How To Run

### `sample.env`: 
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
  - Clone the directory
  - Enter the project directory with 
    ```bash
    cd stack-overflow-clone
    ```
  - On Macbook:
    - Run 
      ```bash
      touch .env
      ``` 
      This will create a file called `.env`.
      - It might help to have a look at our `sample.env` file to get an idea of the environment variables you need.
      - You can run 
        ```bash
        vim sample.env
        ``` 
        in the same directory to have a look. You can exit it with `:q`.
    - Run 
      ```bash
      vim .env
      ``` 
      Now we can edit the file line by line. When done, enter `:wq` to write and quit the file.
  - On Windows:
    - Run 
      ```bash
      copy con .env
      ``` 
      This will let you start writing your environment variable values. I'd recommend opening the sample `.env` file in a separate terminal tab or referring above for the values it should contain.
    - Once you have set all the values, hit `CRTL+Z` to save.
  - Now that our environment variables are done, run 
    ```bash
    cd Backend
    ``` 
    and then run 
    ```bash
    cd BackendServer
    ``` 
    to enter the correct directory.
  - Here, we will run 
    ```bash
    dotnet ef migrations add InitialCreate
    ``` 
    This will create a migration script to run when you start the project.
  - Finally, we can run 
    ```bash
    docker compose up --build
    ``` 
    to get the Docker container running.
    - Note: The way the project is set up on the repo, it expects the backend on port `8080` and the frontend on port `5173`. If you wish to change these, you need to manually modify the frontend `Dockerfile` and `vite.config.js`, otherwise, enjoy! The only port that needs to open on your machine is `5173`.

- In an IDE
  - Clone the directory
  - Enter the project and create a new `.env` file in the main directory that contains both the `Backend` and `Frontend` directories.
  - Copy and paste the values from the `sample.env` into the new `.env`, changing anything in `{}`.
  - In your own terminal or the IDE's built-in interface, run 
    ```bash
    cd Backend
    ``` 
    and then 
    ```bash
    cd BackendServer
    ``` 
    and finally 
    ```bash
    dotnet ef migrations add InitialCreate
    ``` 
    to create a migration.
  - Finally, run 
    ```bash
    docker compose up --build
    ```
    - Note: The way the project is set up on the repo, it expects the backend on port `8080` and the frontend on port `5173`. If you wish to change these, you need to manually modify the frontend `Dockerfile` and `vite.config.js`, otherwise, enjoy! The only port that needs to open on your machine is `5173`.

## Contributors
- matezalantoth
- qeed97
- BalintCoder
