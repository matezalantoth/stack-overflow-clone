import {useEffect, useState} from "react";
import ProfileRenderer from "./ProfileRenderer.jsx";
import {useParams} from "react-router-dom";

export default function PublicUser ()
{
    const[user, setUser] = useState(null)
    const {userName} = useParams()
    useEffect(() => {
        const fetchUserByUserName = async () => {
            try {
                const res = await fetch(`/api/getUserByUserName?userName=` + userName)
                setUser(await res.json())
            } catch (error) {
                console.log(error)
            }
        }
        
        fetchUserByUserName()
        
    }, []);
    
    return <ProfileRenderer user={user}/>
}