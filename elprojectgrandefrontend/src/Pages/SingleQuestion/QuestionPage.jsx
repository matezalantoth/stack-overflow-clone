import {useParams} from "react-router-dom";
import {useEffect, useState} from "react";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";
import QuestionComponent from "./QuestionComponent.jsx";
import {AnswerComponent} from "./AnswerComponent.jsx";
import {MessageComponent} from "./MessageComponent.jsx";
import {fetchAnswers} from "./QuestionPageServiceProvider.jsx";

export default function QuestionPage() {
    const [cookies] = useCookies(['user']);
    const {questionId} = useParams();
    const [questionData, setQuestionData] = useState(null);
    const [answers, setAnswers] = useState(null);
    const [user, setUser] = useState(null);
    const [reply, setReply] = useState({content: ""});
    const [submittable, setSubmittable] = useState(false);
    const [isAdmin, setIsAdmin] = useState(null);
    const [renderForm, setRenderForm] = useState(false);
    const urlParams = new URLSearchParams(window.location.search);
    const state = urlParams.get('state');
    const showErrorToast = (message) => toast.error(message);
    useEffect(() => {
        const fetchQuestion = async () => {
            try {
                const res = await fetch('/api/Questions/' + questionId, {
                    headers: {
                        'Authorization': "Bearer " + cookies.user
                    }
                });
                const data = await res.json();
                setQuestionData(() => data);
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
        fetchQuestion();
        fetchAnswers(questionId, setAnswers);

    }, []);

    useEffect(() => {
        console.log(isAdmin);
        console.log(state);
        if (isAdmin && state === "edit") {
            setRenderForm(true);
        }
    }, [isAdmin])

    useEffect(() => {
        if (reply.content.length > 0) {
            setSubmittable(true)
            return;
        }
        setSubmittable(false);
    }, [reply])

    return questionData && answers && user ? (
        <>
            <QuestionComponent question={questionData} setQuestion={setQuestionData} isAdmin={isAdmin} user={user}
                               renderForm={renderForm} setRenderForm={setRenderForm}/>
            <div>
                {
                    answers.map(answer => {
                        return (
                            <AnswerComponent key={answer.id} answer={answer} question={questionData} user={user}
                                             cookies={cookies}
                                             setAnswers={setAnswers}
                                             setUser={setUser} setQuestionData={setQuestionData}/>
                        )
                    })
                }
            </div>
            <MessageComponent questionId={questionId} reply={reply} submittable={submittable} cookies={cookies}
                              setReply={setReply}
                              setAnswers={setAnswers} setSubmittable={setSubmittable}/>
        </>
    ) : <div className="text-center text-blue-500">Loading...</div>;
}
