import {useEffect, useState} from "react";
import QuestionsElement from "./QuestionsElement.jsx";
import AnswersElement from "./AnswersElement.jsx";
import {useCookies} from 'react-cookie';
import {useNavigate} from 'react-router-dom';
import ProfileRenderer from "./ProfileRenderer.jsx";


export default function ProfilePage ()
{
    const [user, setUser] = useState(null)
    const navigate = useNavigate();
    const[cookies] = useCookies(['user'])


    useEffect(() => {
        const fetchUsers = async () => {
            try{
                const res = await fetch(`/api/Users`,
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

    }, [cookies])

    
    return  user ? (<>
        <div className="user bg-white p-6 rounded-lg shadow-lg max-w-sm ml-0">
            <p className="border-amber-500 border-4 w-full p-2 rounded-md mb-3 bg-amber-50 text-lg font-semibold">
                Name: {user.name}
            </p>
            <p className="border-amber-700 border-4 w-full p-2 rounded-md mb-3 bg-amber-100 text-lg font-semibold">
                Email: {user.email}
            </p>
            <p className="border-cyan-600 border-4 w-full p-2 rounded-md mb-3 bg-cyan-50 text-lg font-semibold">
                Username: {user.userName}
            </p>
            <button
                className="underline text-blue-500 mt-4 text-lg font-medium hover:text-blue-700"
                onClick={() => navigate('/editPage')}
            >
                Edit
            </button>
        </div>
        <ProfileRenderer user={user}/> </>):
            <></>
        }