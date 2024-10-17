import React, {useEffect, useState} from 'react';
import Tag from "../../components/Tag/Tag.jsx";
import {useNavigate, useParams} from "react-router-dom";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";

export default function TagPage() {
    const navigate = useNavigate();
    const [cookies] = useCookies(['user']);
    const {tagId} = useParams();
    const [user, setUser] = useState(null);
    const [tagData, setTagData] = useState(null);
    const [submittable, setSubmittable] = useState(false);
    const [isAdmin, setIsAdmin] = useState(null);
    const [renderForm, setRenderForm] = useState(false);
    const [editingQuestionId, setEditingQuestionId] = useState(null);
    const urlParams = new URLSearchParams(window.location.search);
    const state = urlParams.get('state');
    const showErrorToast = (message) => toast.error(message);

    useEffect(() => {
        const fetchTag = async () => {
            try{
                const res = await fetch('/api/Tags/' + tagId, {
                    headers: {
                        'Authorization': "Bearer " + cookies.user
                    }
                });
                const data = await res.json();
                setTagData(() => data);
            } catch (error) {
                console.log(error);
            }
        }
        const fetchUser = async () => {
            try {
                const res = await fetch('/api/Users/GetBySessionToken', {
                    headers: {'Authorization': "Bearer " + cookies.user}
                })
                setUser(await res.json());
            } catch (e) {
                console.log(e);
                showErrorToast("Something went wrong")
            }
        }
        const checkIfAdmin = async () => {

            const res = await fetch('/api/Users/IsUserAdmin', {
                headers: {'Authorization': "Bearer " + cookies.user}
            })

            const data = await res.json();
            setIsAdmin(data);
        }
        checkIfAdmin();
        fetchUser();
        fetchTag();
    },[]);

    useEffect(() => {
        console.log(isAdmin);
        console.log(state);
        if (isAdmin && state === "edit") {
            setRenderForm(true);
        }
        if (isAdmin && state === "questEdit") {
            setEditingQuestionId(urlParams.get("id"));
        }
    }, [isAdmin])

    return (
        <div className>
            <div className="text-center">
                <h1>{tagData.tagName}</h1>
                <label>{tagData.description}</label>
            </div>
            <div
                className="text-center w-120 flex justify-center items-center flex-col mt-4">
                {tagData.questions.map((question) => (
                    <div
                        className='relative h-20 text-black border-b-2 border-l-2 rounded hover:bg-gray-100 transition w-120 overflow-hidden'
                    >
                        <h2 className='text-blue-700 cursor-pointer ml-2 break-words' onClick={() => {
                            navigate(`/question/${question.id}`);
                        }}>{question.title}</h2>
                        <p className='text-xs line-clamp-3 text-gray-600 leading-4 break-words ml-2'>{question.content}</p>
                        <div className="flex flex-row gap-3 justify-center">
                            {question.tags.map((tag) => {
                                return (<Tag key={tag} tag={tag}/>)
                            })}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

