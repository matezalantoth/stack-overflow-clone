import React from 'react';
import { CopyToClipboard } from 'react-copy-to-clipboard';

export default function ShareButton() {

    const [isOpen, setIsOpen] = React.useState(false);

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
                <CopyToClipboard text={url}>
                    <div>
                        <textarea className="border h-8 w-3/4 resize-none cursor-not-allowed rounded-md" readOnly={true} value={url} />
                        <button>Copy URL to the clipboard</button>
                    </div>
                </CopyToClipboard>
            </div>)}
        </div>
    );
}