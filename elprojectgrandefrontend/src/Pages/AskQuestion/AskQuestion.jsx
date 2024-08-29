import {useNavigate} from "react-router-dom";
import {useEffect, useState} from "react";
import {toast} from "react-hot-toast";

export default function AskQuestion(props) {
    const { cookies } = props;
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
        <div className='flex-col justify-center'>
            <h1 className='text-xl font-medium text-gray-900'>
                Ask Question
            </h1>
            <form className='space-y-6' action='#'>
                <div className='flex flex-col justify-center border-2 border-black rounded-lg shadow sm:p-6 md:p-8'>
                    <label className='block mb-2 text-sm font-medium text-gray-900'>
                        Title
                    </label>
                    <input onChange={(event) => {
                        setQuestion({...question, title: event.target.value});
                    }}
                           type='text'
                           required
                           placeholder='Title'
                           className='bg-white border-2 border-black rounded-lg shadow'
                    />
                </div>
                <div className='flex flex-col justify-center border-2 border-black rounded-lg shadow sm:p-6 md:p-8'>
                    <label className='block mb-2 text-sm font-medium text-gray-900'>
                        Details of your question
                    </label>
                    <div className='relative overflow-auto w-full flex flex-col'>
                        <input onChange={(event) => {
                            setQuestion({...question, content: event.target.value});
                        }}
                               type='text'
                               required
                               placeholder='question'
                               className='bg-white border-2 border-black rounded-md shadow'
                        />
                    </div>
                </div>
                <button className='block mb-2 text-sm font-medium text-blue-500 border border-blue-500 rounded-lg shadow sm:p-6 md:p-8'
                onClick={(event) => {
                    event.preventDefault();
                    createQuestion(event).then((data) => {
                        if (data.message) {
                            showErrorToast(data.message);
                        }
                    });
                }}>
                   submit
                </button>
            </form>
        </div>
    );
}