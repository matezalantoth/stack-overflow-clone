import React from 'react';
import {useNavigate} from "react-router-dom";

export default function Tag({tag}) {
    const navigate = useNavigate();

    return (
        <div>
            <label className={"text-xs rounded bg-gray-300 p-1"} onClick={() => navigate("/tag/"+ tag.id)}>{tag.tagName}</label>
        </div>
    );
}

