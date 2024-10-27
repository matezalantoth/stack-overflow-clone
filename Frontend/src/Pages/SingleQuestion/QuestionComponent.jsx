/* eslint-disable react/prop-types */
import {useNavigate} from "react-router-dom";
import {formatTimeDifference} from "./QuestionPageServiceProvider.jsx";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faFeather, faX} from "@fortawesome/free-solid-svg-icons";
import {QuestionForm} from "./QuestionForm.jsx";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";
import ShareButton from "../../components/ShareButton/ShareButton.jsx";


export default function QuestionComponent({
                                              question, setQuestion, isAdmin, user, renderForm, setRenderForm
                                          }) {
    const navigate = useNavigate();
    const [cookies] = useCookies(['user']);

    const deleteQuestion = async (id) => {
        const res = await fetch('/api/questions/' + id, {
            method: 'DELETE',
            headers: {
                'Authorization': "Bearer " + cookies.user
            }
        })
        const data = await res.json()
        if (data.message) {
            toast.error(data.message);
            return;
        }
        navigate('/');
    }


    return (
        <div className="w-3/4 min-h-40 mx-auto mt-12 p-6"> {
            renderForm ? <QuestionForm question={question} setQuestion={setQuestion} setRenderForm={setRenderForm}/> :
                <div
                >

                    <div
                        className="animate-border rounded-md bg-white bg-gradient-to-r from-blue-400 to-green-700 p-1 h-full w-full"
                    >
                        <div className="bg-white rounded-lg shadow-md">
                            <div className="ml-4">
                            <span
                                onClick={() => {
                                    navigate("/user/" + question.username);
                                }}
                                className="text-xs text-gray-500 hover:underline cursor-pointer"
                            >
                                {question.username}
                                </span>
                                <span className="text-xs text-gray-500">
                                {" "}
                                    | {formatTimeDifference(question.postedAt)}
                            </span>
                                {isAdmin || user.userName === question.username ?
                                    <>
                                        <button onClick={() => setRenderForm(true)} className="m-2 text-blue-400"
                                                title="Edit">
                                            <FontAwesomeIcon
                                                icon={faFeather}/>
                                        </button>
                                        <button onClick={() => deleteQuestion(question.id)}
                                                className="text-red-500 float-right mr-4 mt-2"
                                                title="Delete"><FontAwesomeIcon icon={faX}/>
                                        </button>
                                    </> : <></>}
                                <h1 className="text-2xl font-bold text-blue-500 mb-4 break-words">
                                    {question.title}
                                </h1>
                                <div className="text-black break-words">{question.content}</div>
                                <ShareButton/>
                            </div>
                        </div>

                    </div>
                </div>}
        </div>
    )
}