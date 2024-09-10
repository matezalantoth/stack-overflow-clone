import {useNavigate} from "react-router-dom";
import {useEffect, useState} from "react";
import {toast} from "react-hot-toast";
import {useCookies} from "react-cookie";

export default function AskQuestion() {
    const [cookies] = useCookies(['user']);
    const navigate = useNavigate();
    const [question, setQuestion] = useState({});
    const [submittable, setSubmittable] = useState(false);
    const showErrorToast = (data) => toast.error(data.message);

    useEffect(() => {
        if (
            question.title &&
            question.content
        ) {
           setSubmittable(true);
        }
    }, [question]);

    useEffect(() => {
        if(!cookies.user){
            navigate('/login');
        }
    }, [cookies])

    const createQuestion = async () => {
        const res = await fetch('/api/Questions',{
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': cookies.user
            },
            body: JSON.stringify({
                ...question,
                sessionToken: cookies.user,
                postedAt: new Date(Date.now()).toISOString()
            }),
        });
        const data = await res.json();
        return data;
    }

    return (
        <div className="flex flex-col items-center justify-center mt-24 p-4">
            <div className="w-full max-w-lg bg-white rounded-lg shadow-lg p-6">
                <h1 className="text-2xl font-semibold text-gray-900 mb-6 text-center">
                    Ask a Question
                </h1>
                <form className="space-y-6" action="#">
                    <div className="flex flex-col">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Title
                        </label>
                        <textarea
                            onChange={(event) => {
                                setQuestion({...question, title: event.target.value});
                            }}
                            required
                            placeholder="Enter your question title"
                            className="border border-gray-300 rounded-lg px-4 py-2 resize-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        ></textarea>
                    </div>

                    <div className="flex flex-col">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Details of your question
                        </label>
                        <textarea
                            onChange={(event) => {
                                setQuestion({...question, content: event.target.value});
                            }}
                            required
                            placeholder="Provide more details about your question"
                            className="border border-gray-300 rounded-lg px-4 py-2 h-32 resize-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        ></textarea>
                    </div>

                    <button
                        className="w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 rounded-lg transition duration-200 shadow-md focus:ring-2 focus:ring-blue-300"
                        onClick={(event) => {
                            event.preventDefault();
                            createQuestion(event).then((data) => {
                                if (data.message) {
                                    showErrorToast(data.message);
                                }
                            });
                        }}
                    >
                        Submit
                    </button>
                </form>
            </div>
        </div>
    );
}