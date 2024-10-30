import React, {useEffect, useState} from 'react';
import Tag from "../../components/Tag/Tag.jsx";
import {useNavigate, useParams} from "react-router-dom";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";
import QuestionComponent from "../SingleQuestion/QuestionComponent.jsx";
import {QuestionsComponent} from "../WelcomePage/QuestionsComponent.jsx";
import {CheckIfSessionExpired} from "../../CheckIfSessionExpired.jsx";
import TagComponent from "./TagComponent.jsx";
import TagForm from "./TagForm.jsx";

export default function TagPage({setUserLoginCookies}) {
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
                if (data.message) {
                    showErrorToast(data.message);
                    return;
                }
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
                const data = await res.json();
                if (data.message) {
                    showErrorToast(data.message);
                    return;
                }
                setUser(data);
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
            if (data.message) {
                showErrorToast(data.message);
                return;
            }
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

    CheckIfSessionExpired(setUserLoginCookies);

    return tagData && (<div> {
        renderForm ? <TagForm tag={tagData} setTag={setTagData} setRenderForm={setRenderForm}/> :
        <TagComponent tag={tagData} setTag={setTagData} isAdmin={isAdmin} user={user} renderForm={renderForm} setRenderForm={setRenderForm}/>}
    </div>
    );
}

