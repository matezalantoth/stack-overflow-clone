import {useParams} from "react-router-dom";
import {useEffect, useState} from "react";

export default function QuestionPage ()
{
    
const useParams = useParams()
    
    const[questionData, setQuestionData] = useState(null)
    
    
        useEffect(() =>{
            const fetchQuestions = async () => {
               try {
                   
                   const res = fetch('http://localhost:5212/Questions')
                   console.log(res)
                   const data = await res.json()
                   setQuestionData(data)
               } catch (error) {
                   console.log(error)
               }
            }
            fetchQuestions();
        }, [])
        
        
        return (
            
            <div className="Question-Holder">
                {questionData.map(question => (
                    <p>{question.content}</p>
                ))}
            </div>
        )
        
        
        
    
}