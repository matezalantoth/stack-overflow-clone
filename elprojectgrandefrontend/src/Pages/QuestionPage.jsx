import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useCookies } from "react-cookie";

export default function QuestionPage() {
    const [cookies] = useCookies(['user']);
    const { questionId } = useParams();
    const [questionData, setQuestionData] = useState(null);
    const [answers, setAnswers] = useState(null);
    const [reply, setReply] = useState({content: ""});
    const [submittable, setSubmittable] = useState(false);

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
        const fetchAnswers = async () => {
            try {
                const res = await fetch('/api/Answers?questionId=' + questionId);
                const data = await res.json();
                setAnswers(data);
            } catch (error){
                console.log(error);
            }
        }
        fetchQuestion();
        fetchAnswers();
        }, []);

    useEffect(() => {
        console.log(reply.content)
        if(reply.content.length > 0){
            setSubmittable(true)
            return;
        }
        setSubmittable(false);
    }, [reply])
    return questionData && answers ? (
        <>
            <div className="w-3/4 mx-auto mt-12 p-6 bg-white rounded-lg shadow-md">
                <h1 className="text-2xl font-bold text-blue-500 mb-4 break-words">{questionData.title}</h1>
                <div className="text-black break-words">{questionData.content}</div>
            </div>
        <div className="w-3/4 mx-auto p-4 bg-white shadow-lg rounded-lg mt-2">
            <form>
                <div className="mb-4">
                    <label  className="block text-gray-700 text-sm font-bold mb-2">Your Reply</label>
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
                        disabled={!submittable}
                        type="submit"
                        className="disabled:bg-gray-400 bg-blue-500 text-white font-bold py-2 px-4 rounded-lg hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-400 focus:ring-opacity-50"
                    >
                        Submit
                    </button>
                </div>
            </form>
        </div></>


) : <div className="text-center text-blue-500">Loading...</div>;
}
