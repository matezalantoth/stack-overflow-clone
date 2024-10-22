import {useNavigate} from "react-router-dom";

export default function QuestionsElement ({questions})
{
    const navigate = useNavigate()
    return (
        
        <div id="questionHolder">

            {/* eslint-disable-next-line react/prop-types */}
            {questions.map(question => {
                console.log(question)
                // eslint-disable-next-line react/jsx-key
                return (<div><div className='hover:underline text-blue-600' onClick={() => navigate('/question/'+ question.id) }>{question.title}</div><div>{question.postedAt}</div></div>)
            })}
            
        </div>
        
        
        
    )
}