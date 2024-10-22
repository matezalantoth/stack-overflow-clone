import {useEffect, useState} from "react";
import ProfileRenderer from "./ProfileRenderer.jsx";
import {useParams} from "react-router-dom";
import {toast} from "react-hot-toast";

export default function PublicUser() {
    const [user, setUser] = useState(null)
    const {userName} = useParams()
    useEffect(() => {
        const fetchUserByUserName = async () => {
            try {
                const res = await fetch(`/api/getUserByUserName?userName=` + userName)
                const data = await res.json()
                if (data.message) {
                    toast.error(data.message)
                    return;
                }
                setUser(data)
            } catch (error) {
                console.log(error)
            }
        }

        fetchUserByUserName()

    }, []);

    return <ProfileRenderer user={user}/>
}