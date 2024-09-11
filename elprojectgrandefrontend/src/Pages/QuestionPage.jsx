import {useNavigate, useParams} from "react-router-dom";
import {useEffect, useState} from "react";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faArrowDown, faArrowUp, faCheck} from "@fortawesome/free-solid-svg-icons";

export default function QuestionPage() {
    const [cookies] = useCookies(['user']);
    const {questionId} = useParams();
    const [questionData, setQuestionData] = useState(null);
    const [answers, setAnswers] = useState(null);
    const [user, setUser] = useState(null);
    const [reply, setReply] = useState({content: ""});
    const [submittable, setSubmittable] = useState(false);
    const navigate = useNavigate();
    const showErrorToast = (message) => toast.error(message);
    const showSuccessToast = (message) => toast.success(message);

    useEffect(() => {
        const fetchQuestion = async () => {
            try {
                const res = await fetch('/api/Questions/' + questionId, {
                    headers: {
                        'Authorization': cookies.user
                    }
                });
                const data = await res.json();
                setQuestionData(data);
            } catch (error) {
                console.log(error);
            }
        }
        const fetchUser = async () => {
            try {
                const res = await fetch('/api/Users/GetBySessionToken', {
                    headers: {'Authorization': cookies.user}
                })
                setUser(await res.json());
            } catch (e) {
                console.log(e);
                showErrorToast("Something went wrong")
            }
        }
        fetchUser();
        fetchQuestion();
        fetchAnswers();
    }, []);

    const fetchAnswers = async () => {
        try {
            const res = await fetch('/api/Answers?questionId=' + questionId);
            const data = await res.json();
            setAnswers(data);
        } catch (error) {
            console.log(error);
        }
    }

    const postAnswer = async () => {
        try {
            const res = await fetch('/api/Answers?questionId=' + questionId, {
                method: "POST",
                headers: {
                    'Authorization': cookies.user,
                    'Content-type': 'application/json'
                },
                body: JSON.stringify({content: reply.content, postedAt: new Date(Date.now()).toISOString()}),
            });
            return await res.json();
        } catch (error) {
            showErrorToast("something went wrong");
            console.error(error);
        }
    }

    useEffect(() => {
        if (reply.content.length > 0) {
            setSubmittable(true)
            return;
        }
        setSubmittable(false);
    }, [reply])

    const formatTimeDifference = (postedAt) => {
        const now = new Date();
        const tempPostedAt = new Date(postedAt);
        let offset = tempPostedAt.getTimezoneOffset() * -1;
        const postedDate = new Date(tempPostedAt.getTime() + offset * 60000);
        let differenceInSeconds = Math.floor(((now - postedDate) / 1000));
        if (differenceInSeconds < 60) {
            return `${differenceInSeconds} seconds ago`;
        } else if (differenceInSeconds < 3600) {
            const minutes = Math.floor(differenceInSeconds / 60);
            return `${minutes} minutes ago`;
        } else if (differenceInSeconds < 86400) {
            const hours = Math.floor(differenceInSeconds / 3600);
            return `${hours} hours ago`;
        }
        const days = Math.floor(differenceInSeconds / 86400);
        return `${days} days ago`;

    };

    const handleAccept = async (id) => {
        const res = await fetch('/api/accept/' + id, {
            method: 'POST',
            headers: {
                Authorization: cookies.user
            }
        });
        const data = await res.json();
        if (data.question) {
            setQuestionData({...questionData, hasAccepted: true});
            const updatedAnswers = answers.map(ans => {
                if (ans.id === data.id) {
                    return {...ans, accepted: true}
                }
                return ans;
            })
            setAnswers(updatedAnswers);
        }
    }

    const sendUpvote = async (id) => {
        const res = await fetch('/api/Answers/' + id + '/upvote', {
            method: 'PATCH',
            headers: {
                Authorization: cookies.user
            }
        })
        await res.json();
    }


    const handleUpvote = async (id) => {
        try {
            sendUpvote(id)
        } catch (error) {
            console.log(error);
            showErrorToast("Something went wrong")
            return;
        }
        setAnswers(answers.map(ans => {
            if (ans.id === id) {
                ans.votes++;
            }
            return ans;
        }))
    }

    const sendDownvote = async (id) => {

        const res = await fetch('/api/Answers/' + id + '/downvote', {
            method: 'PATCH',
            headers: {
                Authorization: cookies.user
            }
        })
        await res.json();

    }

    const handleDownvote = async (id) => {
        try {
            sendDownvote(id);
        } catch (e) {
            console.log(e)
            showErrorToast("Something went wrong")
            return;
        }
        setAnswers(answers.map(ans => {
            if (ans.id === answer.id) {
                ans.votes--;
            }
            return ans;
        }))

    }

    const checkIfUpvoted = (id) => {
        return user.upvotes.some(identifier => identifier === id);
    }
    const checkIfDownvoted = (id) => {
        return user.downvotes.some(identifier => identifier === id);
    }

    const handleUpvoting = (answer) => {
        if (!checkIfUpvoted(answer.id)) {
            handleUpvote(answer.id);
            setUser({
                ...user,
                upvotes: [...user.upvotes, answer.id],
                downvotes: user.downvotes.filter(ide => ide.id !== answer.id)
            })
            return;
        }
        sendUpvote(answer.id);
        setAnswers(answers.map(ans => {
            console.log(ans.id === answer.id)
            if (ans.id === answer.id) {
                ans.votes -= 1;

            }
            return ans;
        }));
        setUser({
            ...user,
            upvotes: user.upvotes.filter(answerId => answerId !== answer.id)
        });
    }

    const handleDownvoting = (answer) => {
        if (!checkIfDownvoted(answer.id)) {
            handleDownvote(answer.id);
            setUser({
                ...user,
                downvotes: [...user.downvotes, answer.id],
                upvotes: user.upvotes.filter(ans => ans.id !== answer.id)
            });
            return;
        }
        sendDownvote(answer.id);
        setAnswers(answers.map(ans => {
            if (ans.id === answer.id) {
                ans.votes++;
            }
            return ans;
        }))
        setUser({
            ...user,
            downvotes: user.downvotes.filter(ans => ans.id !== answer.id)
        });
    }

    return questionData && answers && user ? (
        <>
            <div className="w-3/4 mx-auto mt-12 p-6 bg-white rounded-lg shadow-md">
                <span onClick={() => {
                    navigate("/user/" + questionData.username)
                }}
                      className="text-xs text-gray-500 hover:underline cursor-pointer">{questionData.username}</span><span
                className="text-xs text-gray-500 cursor-pointer"> | {formatTimeDifference(questionData.postedAt)}</span>
                <h1 className="text-2xl font-bold text-blue-500 mb-4 break-words">{questionData.title}</h1>
                <div className="text-black break-words">{questionData.content}</div>
            </div>
            {
                answers.map(answer => {
                    return (
                        <div
                            className="w-3/4 min-h-40 mt-12 p-6 bg-white rounded-lg shadow-md block m-auto">
                            <div className="flex">
                                <div className="w-1/12 border-r-2 justify-between">
                                    <div
                                        onClick={() => {
                                            handleUpvoting(answer);
                                        }}
                                        className={'text-center text-3xl hover:text-black transition block cursor-pointer ' + (checkIfUpvoted(answer.id) ? '' : 'text-gray-400')}>
                                        <FontAwesomeIcon icon={faArrowUp}/>
                                    </div>
                                    <div
                                        className="text-center text-3xl transition block">
                                        {answer.votes}
                                    </div>
                                    <div
                                        onClick={() => {
                                            handleDownvoting(answer);
                                        }}
                                        className={"text-center text-3xl text-gray-400 hover:text-black transition block cursor-pointer " + (checkIfDownvoted(answer.id) ? '' : 'text-gray-400')}>
                                        <FontAwesomeIcon icon={faArrowDown}/>
                                    </div>
                                    {user.userName === questionData.username && !questionData.hasAccepted ? <div
                                        className="text-center text-3xl text-gray-400 hover:text-green-500 transition block cursor-pointer">
                                        <FontAwesomeIcon onClick={() => {
                                            handleAccept(answer.id)
                                        }} icon={faCheck}/>
                                    </div> : <></>}
                                    {questionData.hasAccepted ? <div
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
                                    <div className="text-black break-words whitespace-normal">
                                        {answer.content}
                                    </div>

                                </div>
                            </div>

                        </div>
                    )
                })
            }
            <div className="w-3/4 mx-auto p-4 bg-white shadow-lg rounded-lg mt-2">
                <form>
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">Your Reply</label>
                        <textarea
                            onChange={(event) => {
                                setReply({...reply, content: event.target.value});
                            }}
                            id="reply"
                            rows="5"
                            className="resize-none w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
                            placeholder="Write your reply here..."
                        ></textarea>
                    </div>

                    <div className="flex justify-end">
                        <button
                            onClick={async (event) => {
                                event.preventDefault();
                                const data = await postAnswer();
                                if (!data.content) {
                                    showErrorToast("Something went wrong");
                                    return;
                                }
                                showSuccessToast("Successfully posted answer!");
                                fetchAnswers();
                                setReply({content: ""});
                                setSubmittable(false);
                            }}
                            disabled={!submittable}
                            type="submit"
                            className="disabled:bg-gray-400 bg-blue-500 text-white font-bold py-2 px-4 rounded-lg hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-400 focus:ring-opacity-50"
                        >
                            Submit
                        </button>
                    </div>
                </form>
            </div>
        </>
    ) : <div className="text-center text-blue-500">Loading...</div>;
}
