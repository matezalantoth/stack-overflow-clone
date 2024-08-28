
import {Component} from "react";
import Questions from "../../components/questions/Questions.jsx";

export default function WelcomePage() {

    return (
        <div className="welcomePage relative flex justify-center items-center flex-col">
            <h1 className='menj'>HELLO THERE</h1>

            <h3 className='innen'>Questions</h3>
            <Questions />
        </div>
    )
}