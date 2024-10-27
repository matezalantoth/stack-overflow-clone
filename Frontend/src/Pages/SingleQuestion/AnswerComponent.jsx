/* eslint-disable react/prop-types */
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faArrowDown, faArrowUp, faCheck, faFeather, faX} from "@fortawesome/free-solid-svg-icons";
import {
    checkIfDownvoted,
    checkIfUpvoted,
    formatTimeDifference,
    handleAccept,
    handleDownvoting,
    handleUpvoting
} from "./QuestionPageServiceProvider.jsx";
import {toast} from "react-hot-toast";
import {useNavigate} from "react-router-dom";
import {useState} from "react";
import {AnswerForm} from "./AnswerForm.jsx";

export const AnswerComponent = ({
                                    answer,
                                    question,
                                    user,
                                    cookies,
                                    setAnswers,
                                    setUser,
                                    setQuestionData,
                                    isAdmin,
                                    editingAnswerId
                                }) => {
    const showErrorToast = (message) => toast.error(message);
    const navigate = useNavigate();
    const [renderForm, setRenderForm] = useState(editingAnswerId === answer.id);

    const deleteAnswer = async (id) => {
        try {
            const res = await fetch("/api/answers/" + id, {
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + cookies.user
                }
            })
            const data = await res.json();
            if (data.message) {
                showErrorToast(data.message);
            }
        } catch (e) {
            console.error(e);
            setAnswers(prevAnswers => prevAnswers.filter(answer => answer.id !== id));
        }
    }

    return editingAnswerId === answer.id || renderForm ?
        <AnswerForm answer={answer} setRenderForm={setRenderForm} setAnswers={setAnswers}/> : (<div
            key={answer.id}
            className="w-3/4 min-h-40 mt-12 p-6 bg-white rounded-lg shadow-md block m-auto">
            <div className="flex">
                <div className="w-1/12 border-r-2 justify-between">
                    <div
                        onClick={async () => {
                            await handleUpvoting(answer, user, cookies, showErrorToast, setAnswers, setUser);
                        }}
                        className={'text-center text-3xl hover:text-black transition block cursor-pointer ' + (checkIfUpvoted(answer.id, user) ? '' : 'text-gray-400')}>
                        <FontAwesomeIcon icon={faArrowUp}/>
                    </div>
                    <div
                        className="text-center text-3xl transition block">
                        {answer.votes}
                    </div>
                    <div
                        onClick={async () => {
                            await handleDownvoting(answer, user, cookies, showErrorToast, setAnswers, setUser);
                        }}
                        className={"text-center text-3xl hover:text-black transition block cursor-pointer " + (checkIfDownvoted(answer.id, user) ? '' : 'text-gray-400')}>
                        <FontAwesomeIcon icon={faArrowDown}/>
                    </div>
                    {user.userName === question.username && !question.hasAccepted ?
                        <div
                            className="text-center text-3xl text-gray-400 hover:text-green-500 transition block cursor-pointer">
                            <FontAwesomeIcon onClick={async () => {
                                await handleAccept(answer.id, cookies, setAnswers, setQuestionData);
                            }} icon={faCheck}/>
                        </div> : <></>}
                    {question.hasAccepted && answer.accepted ? <div
                        className="text-center text-3xl text-green-500 transition block">
                        <FontAwesomeIcon icon={faCheck}/>
                    </div> : <></>}

                </div>
                <div className="w-11/12 pl-6">
                                <span
                                    onClick={() => {
                                        navigate("/user/" + answer.username)
                                    }}
                                    className="text-xs text-gray-500 hover:underline cursor-pointer">{answer.username}</span><span
                    className="text-xs text-gray-500 cursor-pointer"> | {formatTimeDifference(answer.postedAt)}</span>
                    {isAdmin || user.userName === answer.username ?
                        <>
                            <button onClick={() => setRenderForm(true)} className="m-2 text-blue-400"
                                    title="Edit">
                                <FontAwesomeIcon
                                    icon={faFeather}/>
                            </button>
                            <button onClick={() => deleteAnswer(answer.id)}
                                    className="text-red-500 float-right mr-4 mt-2"
                                    title="Delete"><FontAwesomeIcon icon={faX}/>
                            </button>
                        </> : <></>}
                    <div className="text-black break-words whitespace-normal">
                        {answer.content}
                    </div>

                </div>
            </div>

        </div>)
}