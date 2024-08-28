import {useEffect, useState} from "react";
import QuestionsElement from "./QuestionsElement.jsx";
import Questions from "../../components/questions/Questions.jsx";
import AnswersElement from "./AnswersElement.jsx";


export default function ProfilePage ()
{
    const [user, setUser] = useState(null)


    useEffect(() => {
        const fetchUsers = async () => {
            try{
                const res = await fetch(`https://localhost:7223/Users?userId=497836d3-5d7f-4ccb-9ca1-7f8383785022`,
                    {
                        headers : {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
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
    
    console.log(user)
    // console.log(user.username)

    return user ?  (
        <div className="Users">

            <div className="user">
                <p>{user.userName}</p>
            </div>
            <QuestionsElement questions={user.questions}/>
            <AnswersElement answers={user.answers}/>

        </div>
        ) :
        <div>
            loading
        </div>
}