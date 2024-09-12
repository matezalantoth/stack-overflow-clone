
import {Component} from "react";
import Questions from "../../components/questions/Questions.jsx";
import TrendingQuestions from "../../components/TrendingQuestions/TrendingQuestions.jsx";

export default function WelcomePage() {

    return (
        <div className="container flex justify-center items-center relative w-auto mx-auto">
            <div className="welcomePage text-center w-120 flex justify-center items-center flex-col mt-4">
                <Questions />
            </div>
            <div className="trending absolute w-80 top-0 right-0 flex justify-center items-center flex-col mt-4 border-l border-gray-200">
                <p>
                    Trending Questions
                </p>
                <TrendingQuestions />
            </div>
        </div>
    )
}