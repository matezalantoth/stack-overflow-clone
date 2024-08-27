import './Questions.css';
import {useEffect, useState} from "react";

export default function Questions() {
    const [questions, setQuestions] = useState([]);


    useEffect(() => {
        const fetchQuestions = async () => {
            try{
                const res = await fetch("http://localhost:8080/Question" , {
                    method: "GET",
                    headers: {"content-type": "application/json"},
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
            {questions.map((question, index) => (
                <div className="question" key={index} onClick={() => {}}>
                    <p>{question.content}</p>
                </div>
            ))}
        </div>
    ):
        <div>
            Loading
        </div>
}