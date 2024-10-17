/* eslint-disable react/prop-types */
import {useEffect, useState} from "react";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";

export const QuestionForm = ({question, setQuestion, setRenderForm}) => {
    const [inputs, setInputs] = useState(null);
    const [cookies] = useCookies(['user']);
    useEffect(() => {
        setInputs({
            title: question.title, content: question.content
        });
    }, [])

    const updateQuestion = async () => {
        const res = await fetch("/api/questions/" + question.id, {
            method: "PUT",
            body: JSON.stringify(inputs),
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + cookies.user
            }
        })
        const data = await res.json();
        if (data.message) {
            toast.error(data.message);
            return;
        }
        setQuestion(prevQuestion => (
            {...prevQuestion, title: inputs.title, content: inputs.content}
        ));
    }


    return inputs ? (<div
            className="animate-border rounded-md bg-white bg-gradient-to-r from-blue-400 to-green-700 p-1 h-full"
        >
            <div className="bg-white rounded-lg shadow-md pt-10 px-4 h-full">
                <form className="">

                    <input className="text-2xl font-bold text-blue-500 mb-4 h-full w-full"
                           onChange={(e) => {
                               setInputs({...inputs, title: e.target.value})
                           }}
                           defaultValue={question.title}/><br/>
                    <div className="flex flex-col h-full">
                        <textarea
                            className="text-black w-full resize-none border-2 h-full flex-grow"
                            onChange={(e) => {
                                setInputs({...inputs, content: e.target.value});
                                e.target.style.height = 'auto';
                                e.target.style.height = `${e.target.scrollHeight}px`;
                            }}
                        >
        {question.content}
    </textarea>
                    </div>
                    <button className="bg-red-500 rounded-md border-2 mr-2 mt-2 text-white border-red-500 px-2 mb-2"
                            onClick={(e) => {
                                e.preventDefault();
                                setRenderForm(false);
                            }}>Cancel
                    </button>
                    <button onClick={async (e) => {
                        e.preventDefault();
                        await updateQuestion();
                        setRenderForm(false);
                    }} type="submit"
                            className="bg-green-400 text-white rounded-md border-2 border-green-400 px-2 mb-2">Save
                    </button>

                </form>
            </div>

        </div>
    ) : <></>;
}