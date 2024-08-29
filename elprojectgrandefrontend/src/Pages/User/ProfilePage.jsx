import {useEffect, useState} from "react";
import QuestionsElement from "./QuestionsElement.jsx";
import Questions from "../../components/questions/Questions.jsx";
import AnswersElement from "./AnswersElement.jsx";
import {Cookies} from "react-cookie";


export default function ProfilePage ({cookies, setUserLoginCookies})
{
    const [user, setUser] = useState(null)
    const[selectedTab, setSelectedTab] = useState(null)


    useEffect(() => {
        const fetchUsers = async () => {
            try{
                const res = await fetch(`https://localhost:7223/Users`,
                    {
                        headers : {
                            'Authorization': cookies.user
                        }

                    });
                console.log(res)
                const data = await res.json();
                setUser(data);
            } catch (error){
                console.log(error);
            }
        };
        fetchUsers();
    }, []);
    
    console.log(user)
    // console.log(user.username)

    return user ?  (
            <div className="Users">
                
                <div className="user">
                    <p> Name: {user.name}</p>
                    <p> Email: {user.email}</p>
                    <p> UserName: {user.userName}</p>
                </div>

                <div>
                    <span  onClick={() => {setSelectedTab('answer')}}>Answers</span> || <span onClick={() => {setSelectedTab('question')}}>Questions</span>
                </div>
                {selectedTab ? selectedTab === 'question'? 
                    <QuestionsElement questions={user.questions}/> : 
                    <AnswersElement answers={user.answers}/>:<></>}

            </div>
        ) :
        <div>
            loading
            
        </div>
}