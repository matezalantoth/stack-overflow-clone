
import {Component, useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {useCookies} from "react-cookie";

export default function WelcomePage({searchQuestion, normalQuestion, setNormalQuestion, setsearchQuestion } ) {

   
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
                setNormalQuestion(data);
                setsearchQuestion(data)
            } catch (error){
                console.log(error);
            }
        };
        fetchQuestions();
    }, []);
    return searchQuestion ? (
            <div className="welcomePage relative flex justify-center items-center flex-col mt-4">
                
        <div className="questions">
            {searchQuestion.map((question) => {
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
               
            </div>
        ) :
        <div>
            Loading
        </div>
    
    
}