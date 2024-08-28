export default function AnswersElement ({answers})
{
    return  (
        
        <div id= "AnswerHolder" >
            {answers.map(answer => {
                console.log(answer)
                (<div><div>{answer.postedAt}</div><div>{answer}</div></div>)
            })}
        </div>
    )
}