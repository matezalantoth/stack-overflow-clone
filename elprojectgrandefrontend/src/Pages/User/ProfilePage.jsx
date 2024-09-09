import {useEffect, useState} from "react";
import QuestionsElement from "./QuestionsElement.jsx";
import AnswersElement from "./AnswersElement.jsx";
import {useCookies} from 'react-cookie';
import {useNavigate} from 'react-router-dom';


export default function ProfilePage ()
{
    const [user, setUser] = useState(null)
    const[selectedTab, setSelectedTab] = useState(null)
    const navigate = useNavigate();
    const[cookies] = useCookies(['user'])


    useEffect(() => {
        const fetchUsers = async () => {
            try{
                const res = await fetch(`https://localhost:7223/Users`,
                    {
                        headers : {
                            'Authorization': cookies.user
                        }

                    });
                const data = await res.json();
                setUser(data);
            } catch (error){
                console.log(error);
            }
        };
        fetchUsers();
    }, []);

    useEffect(() => {
        if(!cookies.user){
            navigate('/login')
        }
    }, [])

    return user ?  (
            <div className="Users">

                <div className="user">
                    <p>{user.name}</p>
                    <p>{user.email}</p>
                    <p>{user.userName}</p>
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