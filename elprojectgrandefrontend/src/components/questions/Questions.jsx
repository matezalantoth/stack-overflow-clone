
import {useEffect, useState} from "react";

export default function Questions() {
    const [questions, setQuestions] = useState(null);


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