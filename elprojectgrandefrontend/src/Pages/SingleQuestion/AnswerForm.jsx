/* eslint-disable react/prop-types */
import {useCookies} from "react-cookie";
import {useState} from "react";
import {toast} from "react-hot-toast";

export const AnswerForm = ({answer, setRenderForm, setAnswers}) => {

    const [cookies] = useCookies(['user']);
    const [newContent, setNewContent] = useState(answer.content);

    const updateAnswer = async () => {
        const res = await fetch(`/api/answers/${answer.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                Authorization: 'Bearer ' + cookies.user
            },
            body: JSON.stringify(newContent)
        })
        const data = await res.json();
        if (data.message) {
            toast.error(data.message);
            return;
        }
        setAnswers(prevAnswers => prevAnswers.map(a => {
            if (a.id === answer.id) {
                a.content = newContent;
            }
            return a;
        }))
    }
    return (<div
        key={answer.id}
        className="w-3/4 min-h-40 mt-12 p-6 bg-white rounded-lg shadow-md block m-auto">
        <div className="flex">
            <form className="w-full h-full">

                <textarea onChange={(e) => setNewContent(e.target.value)}
                          className="text-black w-full h-full resize-none">
                    {answer.content}
                </textarea>
                <button className="bg-red-500 rounded-md border-2 mr-2 mt-2 text-white border-red-500 px-2 mb-2"
                        onClick={(e) => {
                            e.preventDefault();
                            setRenderForm(false);
                        }}>Cancel
                </button>
                <button onClick={async (e) => {
                    e.preventDefault();
                    await updateAnswer();
                    setRenderForm(false);
                }} type="submit"
                        className="bg-green-400 text-white rounded-md border-2 border-green-400 px-2 mb-2">Save
                </button>
            </form>
        </div>

    </div>)
}