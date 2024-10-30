import React from 'react';
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";
import {useNavigate} from "react-router-dom";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faFeather, faX} from "@fortawesome/free-solid-svg-icons";

export default function TagResultComponent( { id, setSearchResults } ) {

    const [cookies] = useCookies(['user']);
    const showErrorToast = (message) => toast.error(message);
    const navigate = useNavigate();

    const deleteTag = async (id) => {
        const res = await fetch(`/api/tags/${id}`, {
            method: 'DELETE',
            headers: {
                "Authorization": "Bearer " + cookies.user
            }
        })
        try {
            const data = await res.json();
            if (data.message) {
                showErrorToast(data.message);

            }
        } catch (e) {
            console.error(e)
            console.log("setting results to exclude id: " + id);
            setSearchResults(prevRes => prevRes.filter(res => res.id !== id));
        }
    }

    return (
        <>
            <button className="m-2 text-blue-400" onClick={() => navigate("/tag/" + id + "?state=edit")}
                    title="Edit">
                <FontAwesomeIcon
                    icon={faFeather}/>
            </button>
            <button className="text-red-500" title="Delete" onClick={() => deleteTag(id)}><FontAwesomeIcon
                icon={faX}/></button>
        </>
    );
}

