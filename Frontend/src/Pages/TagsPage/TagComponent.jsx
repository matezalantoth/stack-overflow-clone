/* eslint-disable react/prop-types */
import React from 'react';
import {useNavigate} from "react-router-dom";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";
import {QuestionsComponent} from "../WelcomePage/QuestionsComponent.jsx";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faFeather, faX} from "@fortawesome/free-solid-svg-icons";

export default function TagComponent({tag, setTag, isAdmin, user, renderForm, setRenderForm}) {
    const navigate = useNavigate();
    const [cookies] = useCookies(['user']);

    const deleteTag = async (id) => {
        try {
            const res = await fetch('/api/Tags/' + id, {
                method: 'DELETE',
                headers: {
                    'Authorization': "Bearer " + cookies.user
                }
            })
            const data = await res.json();
            if (data.message) {
                toast.error(data.message);
            }
        } catch (e) {
            console.error(e);
            navigate('/');
        }
    }

    return (
        <div className="flex flex-col justify-center items-center">
            <div className="text-center">
                <h1>{tag.tagName}</h1>
                <label>{tag.description}</label>
                {isAdmin  ?
                    <>
                        <button onClick={() => setRenderForm(true)} className="m-2 text-blue-400"
                                title="Edit">
                            <FontAwesomeIcon
                                icon={faFeather}/>
                        </button>
                        <button onClick={() => deleteTag(tag.id)}
                                className="text-red-500 mr-4 mt-2"
                                title="Delete"><FontAwesomeIcon icon={faX}/>
                        </button>
                    </> : <></>}
            </div>
            <div
                className="text-center w-120 flex justify-center items-center flex-col mt-4">
                <QuestionsComponent questions={tag.questions}/>
            </div>
        </div>
    );
}

