import {useParams} from "react-router-dom";
import {useEffect, useState} from "react";

export default function QuestionPage ({cookies})
{
    
    const { questionId} = useParams();
    
    const[questionData, setQuestionData] = useState(null)
    
    
        useEffect(() =>{
            const fetchQuestion = async () => {
               try {
                   
                   const res = await fetch('/api/Questions/'+ questionId, {
                       headers: {
                           'Authorization': cookies.user
                       }
                   })
                   console.log(res)
                   const data = await res.json()
                   setQuestionData(data)
               } catch (error) {
                   console.log(error)
               }
            }
            fetchQuestion();
        }, [])
        console.log(questionData)
        
        return questionData ? (
            
            
            <div id="Question-Holder">
                {questionData.content}
            </div>
        ):
            <></>
        
        
        
    
}