import React, {useEffect, useRef} from 'react';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import {selectLine, selectWord} from "@uiw/react-md-editor";

export default function ShareButton() {

    const [isOpen, setIsOpen] = React.useState(false);
    const textareaRef = useRef(null);

    useEffect(() => {
        if (isOpen && textareaRef.current) {
            textareaRef.current.select();  // Auto-select the textarea content
        }
    }, [isOpen]);

    const toggleOpen = () => {
        setIsOpen(!isOpen);
    };

    const url = window.location.href;

    return (
        <div className="relative">
            <a className="text-xs text-gray-500 hover:underline cursor-pointer" onClick={toggleOpen}>
                share
            </a>
            {isOpen && (
            <div className="absolute min-w-full p-4 mt-2 bg-white rounded shadow">
                <div className="absolute left-1 -top-3
                      border-l-[10px] border-l-transparent
                      border-b-[13px] border-b-white
                      border-r-[10px] border-r-transparent">
                </div>
                <CopyToClipboard text={url}>
                    <div className="flex flex-col justify-between items-center gap-3">
                        <label className="text-black text-lg">Share a link to this question</label>
                        <textarea className="border h-8 w-3/4 resize-none cursor-not-allowed rounded-md" ref={textareaRef} readOnly={true} value={url} />
                        <button className="text-blue-500">Copy URL to the clipboard</button>
                    </div>
                </CopyToClipboard>
            </div>)}
        </div>
    );
}