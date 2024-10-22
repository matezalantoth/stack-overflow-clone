import React, {useEffect, useState} from 'react';
import {useNavigate} from "react-router-dom";
import {useCookies} from "react-cookie";

export default function TrendingQuestions() {
    const [questions, setQuestions] = useState(null);
    const navigate = useNavigate();
    const [cookies] = useCookies(['user']);

    useEffect(() => {
        const fetchQuestions = async () => {
            try {
                const res = await fetch("/api/Questions/trending",
                    {
                        headers: {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        }

                    });
                const data = await res.json();
                setQuestions(data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchQuestions();
    }, []);

    const formatTimeDifference = (postedAt) => {
        const now = new Date();
        const tempPostedAt = new Date(postedAt);
        let offset = tempPostedAt.getTimezoneOffset() * -1;
        const postedDate = new Date(tempPostedAt.getTime() + offset * 60000);
        let differenceInSeconds = Math.floor(((now - postedDate) / 1000));
        if (differenceInSeconds < 60) {
            return `${differenceInSeconds} second${differenceInSeconds === 1 ? '' : 's'} ago`;
        } else if (differenceInSeconds < 3600) {
            const minutes = Math.floor(differenceInSeconds / 60);
            return `${minutes} minute${minutes === 1 ? '' : 's'} ago`;
        } else if (differenceInSeconds < 86400) {
            const hours = Math.floor(differenceInSeconds / 3600);
            return `${hours} hour${hours === 1 ? '' : 's'} ago`;
        }
        const days = Math.floor(differenceInSeconds / 86400);
        return `${days} days ago`;

    };

    return questions ? (
            <div className="trending_questions">
                {questions.map((question) => {
                    return (<div
                        key={question.id}
                        className='relative h-15 pb-1 text-black border-b-2 rounded hover:bg-gray-100 transition w-80 overflow-hidden'>

                        <span className="text-xs ml-2 text-gray-500 hover:underline cursor-pointer">
                            {question.username}{' | '}
                        </span>
                        <span
                            className="text-xs text-gray-500 cursor-pointer">{formatTimeDifference(question.postedAt)}</span>
                        <h2 className='text-blue-700 cursor-pointer ml-2 break-words' onClick={() => {
                            navigate(`/question/${question.id}`);
                        }}>{question.title}</h2>
                        <p className='text-xs line-clamp-1 text-gray-600 leading-4 break-words ml-2'>{question.content}</p>
                    </div>)
                })}
            </div>
        ) :
        <div>
            Loading
        </div>
}