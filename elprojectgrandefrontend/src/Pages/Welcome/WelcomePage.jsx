
import {Component} from "react";
import Questions from "../../components/questions/Questions.jsx";

export default function WelcomePage() {

    return (
        <div className="welcomePage relative flex justify-center items-center flex-col mt-4">
            <Questions />
        </div>
    )
}