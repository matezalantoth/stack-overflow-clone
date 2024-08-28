
export default function QuestionsElement ({questions})
{
    return (
        
        <div id="questionHolder">

            {/* eslint-disable-next-line react/prop-types */}
            {questions.map(question => {
                console.log(question)
                // eslint-disable-next-line react/jsx-key
                return (<div><div>{question.title}</div><div>{question.postedAt}</div></div>)
            })}
            
        </div>
        
        
        
    )
}