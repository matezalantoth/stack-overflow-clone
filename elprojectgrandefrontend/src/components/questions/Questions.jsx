
import {useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {useCookies} from "react-cookie";

export default function Questions() {
    const [questions, setQuestions] = useState(null);
    const navigate = useNavigate();
    const [cookies] = useCookies(['user']);

    useEffect(() => {
        const fetchQuestions = async () => {
             try{
                const res = await fetch("/api/Questions",
                    {
                        headers : {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        }

                    });
                const data = await res.json();
                setQuestions(data);
             } catch (error){
                 console.log(error);
             }
        };
        fetchQuestions();
    }, []);
    console.log(questions)
    return questions ? (
        <div className="questions">
            {questions.map((question) => {
                return (<div
                    className='relative h-20 text-black border-b-2 border-l-2 rounded hover:bg-gray-100 transition w-120 overflow-hidden'

                >
                    <h2 className='text-blue-700 cursor-pointer ml-2 break-words' onClick={() => {
                        navigate(`/question/${question.id}`);
                    }}>{question.title}</h2>
                    <p className='text-xs line-clamp-3 text-gray-600 leading-4 break-words ml-2'>{question.content}</p>
                </div>)
            })}
        </div>
        ) :
        <div>
            Loading
        </div>
}